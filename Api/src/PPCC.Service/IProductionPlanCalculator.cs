using PPCC.Service.Models;
using System.Collections.Generic;

namespace PPCC.Service
{
    /// <summary>
    /// Interface for the calculation of a production plan.
    /// </summary>
    public interface IProductionPlanCalculator
    {
        /// <summary>
        /// Calculates the production plan.
        /// </summary>
        List<PowerPlantProduction> Calculate(double load, List<PowerPlant> powerPlants, Fuels fuels);
    }
}
