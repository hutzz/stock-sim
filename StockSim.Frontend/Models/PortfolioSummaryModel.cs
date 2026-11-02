using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    /// <summary>
    /// Portfolio roll-up from GET /stock/summary. Cash plus market value equals total value,
    /// and realized P&amp;L comes from closed trades while unrealized P&amp;L tracks open positions.
    /// </summary>
    public class PortfolioSummaryModel {
        [JsonPropertyName("cash")]
        public float Cash { get; set; }
        [JsonPropertyName("positions")]
        public int Positions { get; set; }
        [JsonPropertyName("cost_basis")]
        public float CostBasis { get; set; }
        [JsonPropertyName("market_value")]
        public float MarketValue { get; set; }
        [JsonPropertyName("unrealized_pnl")]
        public float UnrealizedPnl { get; set; }
        [JsonPropertyName("realized_pnl")]
        public float RealizedPnl { get; set; }
        [JsonPropertyName("total_value")]
        public float TotalValue { get; set; }
        [JsonPropertyName("total_return")]
        public float TotalReturn { get; set; }
    }
}
