using System.Text.Json.Serialization;

namespace PPCC.Service.Models
{
    public class PowerPlant
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type"), JsonConverter(typeof(JsonStringEnumConverter))]
        public PowerPlantType PowerplantType { get; set; }

        [JsonPropertyName("efficiency")]
        public double Efficiency { get; set; }

        [JsonPropertyName("pmin")]
        public double PMin { get; set; }

        [JsonPropertyName("pmax")]
        public double PMax { get; set; }
    }
}
