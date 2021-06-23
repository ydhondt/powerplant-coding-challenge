using System.Text.Json.Serialization;

namespace PPCC.Service.Models
{
    public class PowerPlantProduction
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("p")]
        public double Power { get; set; }
    }
}
