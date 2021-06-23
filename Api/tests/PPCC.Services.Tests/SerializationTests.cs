using PPCC.Service.Models;
using System.IO;
using System.Text.Json;
using Xunit;

namespace PPCC.Services.Tests
{
    /// <summary>
    /// Series of tests validating whether the different json input files can be processed.
    /// </summary>
    /// <remarks>
    /// Some property names in the JSON file are different from the ones in the model. There is also
    /// an enum which is mapped (ProductionPlantType) and is a potential location for issues.
    /// For ease with this challenge, we just parse the example payloads.
    /// </remarks>
    public class SerializationTests
    {
        [Fact]
        public void Json_Input_Should_Be_Parsed_Correctly()
        {
            // Arrange.
            var data = File.ReadAllText("payload3.json");

            // Act.
            var result = JsonSerializer.Deserialize<Payload>(data);

            // Assert.
            Assert.Equal(910, result.Load);
            // We could do some extra checks here.
        }
    }
}
