using System.Text.Json.Serialization;

namespace PPCC.Service.Models
{
    public class Fuels
    {
        [JsonPropertyName("gas(euro/MWh)")]
        public double GasCost { get; set; }

        [JsonPropertyName("kerosine(euro/MWh)")]
        public double KerosineCost { get; set; }

        [JsonPropertyName("co2(euro/ton)")]
        public double CO2Emission { get; set; }

        [JsonPropertyName("wind(%)")]
        public double WindEfficiency { get; set; }
    }
}
