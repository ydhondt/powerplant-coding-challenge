using System;

namespace PPCC.Service.Models
{
    /// <summary>
    /// The <see cref="EnrichedPowerPlant"/> class is a decorator of a <see cref="Models.PowerPlant"/>.
    /// It provides extra functionality like keeping track of the actual minimum and maximum amount
    /// of MW it can produce, the cost, and what it should produce in a plan.
    /// </summary>
    public class EnrichedPowerPlant
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="EnrichedPowerPlant"/> object based on an
        /// existing <see cref="PowerPlant"/> and <see cref="Fuels"/> combination.
        /// </summary>
        public EnrichedPowerPlant(PowerPlant powerPlant, Fuels fuels)
        {
            // Keep track of the power plant.
            PowerPlant = powerPlant;

            // Calculate the different extra properties.
            CalculateAndSetMaximumGeneratablePower(powerPlant, fuels);
            CalculateAndSetMinimumGeneratablePower(powerPlant, fuels);
            CalculateAndSetCostPerMwh(powerPlant, fuels);
        }

        /// <summary>
        /// Provides access to the <see cref="PowerPlant"/>.
        /// </summary>
        public PowerPlant PowerPlant { get; private set; }

        /// <summary>
        /// Gets the minimum power in MW the plant generates when in use.
        /// </summary>
        public double MinimumGeneratablePower { get; private set; }

        /// <summary>
        /// Gets the maximum power in MW the plant can generate.
        /// </summary>
        public double MaximumGeneratablePower { get; private set; }

        /// <summary>
        /// Gets the cost to produce 1 MWH.
        /// </summary>
        public double CostPerMwh { get; private set; }

        /// <summary>
        /// The amount the plant is actually producing.
        /// </summary>
        public double Producing { get; set; }

        private void CalculateAndSetMaximumGeneratablePower(PowerPlant powerPlant, Fuels fuels)
        {
            MaximumGeneratablePower = powerPlant.PowerplantType == PowerPlantType.WindTurbine ? Math.Round(powerPlant.PMax * fuels.WindEfficiency / 100.0, 1) : powerPlant.PMax;
        }

        private void CalculateAndSetMinimumGeneratablePower(PowerPlant powerPlant, Fuels fuels)
        {
            MinimumGeneratablePower = powerPlant.PowerplantType == PowerPlantType.WindTurbine ? Math.Round(powerPlant.PMax * fuels.WindEfficiency / 100.0, 1) : powerPlant.PMin;
        }

        private void CalculateAndSetCostPerMwh(PowerPlant powerPlant, Fuels fuels)
        {
            // There are only 3 options for now, so no need to go for a switch statement.
            if (powerPlant.PowerplantType == PowerPlantType.WindTurbine)
            {
                CostPerMwh = 0.0;
            }
            else if (powerPlant.PowerplantType == PowerPlantType.GasFired)
            {
                CostPerMwh = fuels.GasCost / powerPlant.Efficiency;
            }
            else
            {
                CostPerMwh = fuels.KerosineCost / powerPlant.Efficiency;
            }
        }
    }
}
