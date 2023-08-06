using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class TokenModel {
        [Required]
        [JsonPropertyName("token")]
        public string? Access { get; set; }
        [Required]
        [JsonPropertyName("refresh")]
        public string? Refresh { get; set; }
    }
}
