using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class BuySellDto {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [Required]
        [JsonPropertyName("quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid quantity.")]
        public int Quantity { get; set; }
    }
}