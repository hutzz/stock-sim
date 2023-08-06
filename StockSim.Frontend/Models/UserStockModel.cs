using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class UserStockModel {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [JsonPropertyName("price")]
        public float Price { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}