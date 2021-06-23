using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PPCC.Service.Models
{
    public class Payload
    {
        [JsonPropertyName("load")]
        public double Load { get; set; }

        [JsonPropertyName("fuels")]
        public Fuels Fuels { get; set; }

        [JsonPropertyName("powerplants")]
        public List<PowerPlant> Powerplants { get; set; }
    }
}
