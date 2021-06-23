using Microsoft.Extensions.Logging.Abstractions;
using PPCC.Service;
using PPCC.Service.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PPCC.Services.Tests
{
    public class ProductionPlanCalculatorTests
    {
        [Fact]
        public void Combined_Generation_Is_Lower_Than_Load()
        {
            // Edge case where the requested load is so high that even when deploying all assests, not
            // enough can be produced. All assets should provide their max output.

            // Arrange.
            var logger = NullLogger<ProductionPlanCalculator>.Instance;
            var load = 1000;
            var plants = new List<PowerPlant>
            {
                new PowerPlant { Efficiency = 1, Name = "x", PMin = 10, PMax = 100, PowerplantType = PowerPlantType.GasFired},
                new PowerPlant { Efficiency = 1, Name = "x", PMin = 10, PMax = 200, PowerplantType = PowerPlantType.GasFired}
            };
            var fuels = new Fuels { GasCost = 1 };

            // Act.
            var productionPlanCalculator = new ProductionPlanCalculator(logger);
            var result = productionPlanCalculator.Calculate(load, plants, fuels);
            var producedPower = result.Sum(p => p.Power);

            // Assert.
            Assert.Equal(300, producedPower);
        }

        [Fact]
        public void Load_Is_Lower_Than_Any_Min_Power()
        {
            // Edge case where the requested load is lower than the minimum power of each plant seperately.
            // All assets should be shut down.

            // Arrange.
            var logger = NullLogger<ProductionPlanCalculator>.Instance;
            var load = 5;
            var plants = new List<PowerPlant>
            {
                new PowerPlant { Efficiency = 1, Name = "x", PMin = 10, PMax = 100, PowerplantType = PowerPlantType.GasFired},
                new PowerPlant { Efficiency = 1, Name = "x", PMin = 10, PMax = 200, PowerplantType = PowerPlantType.GasFired}
            };
            var fuels = new Fuels { GasCost = 1 };

            // Act.
            var productionPlanCalculator = new ProductionPlanCalculator(logger);
            var result = productionPlanCalculator.Calculate(load, plants, fuels);
            var producedPower = result.Sum(p => p.Power);

            // Assert.
            Assert.Equal(0, producedPower);
        }

        // Other test cases could be the provisioned payloads.
    }
}
