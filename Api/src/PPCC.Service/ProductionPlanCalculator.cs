using Microsoft.Extensions.Logging;
using PPCC.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPCC.Service
{
    public class ProductionPlanCalculator : IProductionPlanCalculator
    {
        private readonly ILogger<ProductionPlanCalculator> _logger;

        /// <summary>
        /// Initializes a new instance of a <see cref="ProductionPlanCalculator"/> object.
        /// </summary>
        public ProductionPlanCalculator(ILogger<ProductionPlanCalculator> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>
        /// I would normally use an LP solver for this kind of request. But since it is
        /// requested to not do so alternatives are considered.
        /// </para>
        /// <para>
        /// Another approach would be brute force by considering every possible production
        /// level for every plant. Then take the sum and for each combination that matches
        /// the requested load, calculate the cost. Finally select the combination with the
        /// lowest cost.
        /// For wind turbines there are only two values, zero or their capacity. For the
        /// others there are however more options: (pmax-pmin)*10+2. The multiplication by 10
        /// is due to the fact that production can be done up to 0.1 MW. Even with the limited
        /// number of power plants the number of possible combinations easily reaches a couple
        /// billion.
        /// This approach is only possible if some serious pruning can be done to exclude entire
        /// sub trees of combinations. It could be combined with a time limit to obtain a potentially
        /// non optimal solution.
        /// </para>
        /// <para>
        /// In the end I choose for a basic algorithm which tries to fill up as much as much
        /// as cheap as possible as fast as possible. I build in some intelligence in the
        /// form of the possibility to lower production of already assigned units.
        /// </para>
        public List<PowerPlantProduction> Calculate(double load, List<PowerPlant> powerPlants, Fuels fuels)
        {
            // Enrich the power plants with some extra data to make calculations easier.
            var enrichedPowerPlants = powerPlants.Select(p => new EnrichedPowerPlant(p, fuels));

            // Use the plants in merit order.
            var powerPlantsByMeritOrder = enrichedPowerPlants.OrderBy(p => p.CostPerMwh).ToArray();

            // Start with the full load to produce.
            var loadYetToProduce = load;

            // The potential compensation is the sum of the delta between the minimum and maximum power of all plants used so far.
            // This allows for an easy check if we can compensate in case we would produce to much by activating a plant with a
            // large minimum production to achieve the required load.
            var potentialCompensation = 0.0;

            for (int i = 0; i < powerPlantsByMeritOrder.Length; i++)
            {
                var powerPlant = powerPlantsByMeritOrder[i];

                if (loadYetToProduce <= 0)
                {
                    // The load is reached and therefore the power plant should not be activated.
                    powerPlant.Producing = 0;
                    continue;
                }

                if (powerPlant.MaximumGeneratablePower <= loadYetToProduce)
                {
                    // There is still load to be produced and this is the cheapest plant left, so
                    // try to use it to its full capacity.
                    powerPlant.Producing = powerPlant.MaximumGeneratablePower;

                    loadYetToProduce = loadYetToProduce - powerPlant.MaximumGeneratablePower;
                    potentialCompensation = potentialCompensation + powerPlant.MaximumGeneratablePower - powerPlant.MinimumGeneratablePower;
                    continue;
                }

                if (powerPlant.MinimumGeneratablePower <= loadYetToProduce)
                {
                    // This plant is the cheapest plant left and is capable of producing the
                    // remaining load.
                    powerPlant.Producing = loadYetToProduce;

                    loadYetToProduce = 0;
                    continue;
                }

                if (powerPlant.MinimumGeneratablePower > loadYetToProduce && powerPlant.MinimumGeneratablePower - loadYetToProduce <= potentialCompensation)
                {
                    // There is still load to be produced. However the current plant, which is the
                    // cheapest left, would produce more than allowed even at its lowest setting.
                    // However, it is possible to compensate the load by producing less with one of
                    // the other plants which are already producing.
                    powerPlant.Producing = powerPlant.MinimumGeneratablePower;

                    CompensateOverProduction(powerPlantsByMeritOrder, powerPlant.MinimumGeneratablePower - loadYetToProduce);

                    loadYetToProduce = 0;
                    continue;
                }

                // It is not possible to use this plant in the current configuration, put its potential to 0
                // and hope other plants remain available to cover the load to be produced.
                powerPlant.Producing = 0;
            }

            // All power plants have been assigned what they should produce. However, there is no guarantee
            // that this will actually fit the load exactly. We might be short due to the impossibility of
            // an exact match by the algorithm but also because the total installed capacity is less than the
            // demand or the demand is less than the smallest pmin.
            // There are 3 things we could do:
            //   - throw an exception as the load could not be matched
            //   - return the production we have which will be less than the load
            //   - potentially extend the load with the minimum generatable power which is left causing an overload
            // Choice of options 2 and 3 would depend on the penalties for both scenarios. As no info on the
            // approach is available, option 2 is returned.

            return powerPlantsByMeritOrder.Select(p => new PowerPlantProduction { Name = p.PowerPlant.Name, Power = p.Producing }).ToList();
        }

        private void CompensateOverProduction(EnrichedPowerPlant[] powerPlants, double amount)
        {
            // To compensate the over production at the lowest cost, we need to remove production
            // from the plants where production is the most expensive.
            var powerPlantsByReversedMeritOrder = powerPlants.OrderByDescending(p => p.CostPerMwh).ToArray();

            var toCompensate = amount;

            for (int i = 0; i < powerPlantsByReversedMeritOrder.Length; i++)
            {
                var powerPlant = powerPlantsByReversedMeritOrder[i];

                if (powerPlant.Producing > 0)
                {
                    // Calculate the amount the production of the plant can be lowered.
                    var compensation = Math.Min(toCompensate, powerPlant.Producing - powerPlant.MinimumGeneratablePower);

                    // Adapt the production.
                    powerPlant.Producing = powerPlant.Producing - compensation;
                    toCompensate = toCompensate - compensation;

                    // Once toCompensate becomes zero, we could break out of this loop if we really wanted.
                }
            }
        }
    }
}
