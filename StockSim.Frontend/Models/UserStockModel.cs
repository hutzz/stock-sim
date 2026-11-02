using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class UserStockModel {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [JsonPropertyName("price")]
        public float Price { get; set; }
        [JsonPropertyName("previous_price")]
        public float? PreviousPrice { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("avg_cost")]
        public float? AvgCost { get; set; }
        [JsonPropertyName("unrealized_pnl")]
        public float? UnrealizedPnl { get; set; }

        public float TotalValue => (float)Math.Round(Price * Quantity, 2);

        /// Day-over-day change in unit price; null when no previous close is available.
        public float? Change => PreviousPrice is null ? null : (float)Math.Round(Price - PreviousPrice.Value, 2);
    }
}