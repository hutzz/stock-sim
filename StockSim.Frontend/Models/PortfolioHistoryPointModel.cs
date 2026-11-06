using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    /// <summary>
    /// One stored daily portfolio total from GET /stock/history, oldest first. Values are
    /// what the dashboard showed that day, not today's re-derivation of it.
    /// </summary>
    public class PortfolioHistoryPointModel {
        [JsonPropertyName("date")]
        public string? Date { get; set; }
        [JsonPropertyName("cash")]
        public float Cash { get; set; }
        [JsonPropertyName("market_value")]
        public float MarketValue { get; set; }
        [JsonPropertyName("total_value")]
        public float TotalValue { get; set; }
    }
}
