using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    /// <summary>
    /// One position from GET /stock/get. A holding Yahoo has no settled close for arrives
    /// with no price, and everything derived from that price stays null instead of being
    /// quietly filled in with what the shares cost.
    /// </summary>
    public class UserStockModel {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [JsonPropertyName("price")]
        public float? Price { get; set; }
        // The trading session the price above came from. It is routinely older than today:
        // Yahoo publishes no settled close for an in-progress session, weekends or holidays.
        [JsonPropertyName("price_date")]
        public string? PriceDate { get; set; }
        [JsonPropertyName("previous_price")]
        public float? PreviousPrice { get; set; }
        [JsonPropertyName("previous_price_date")]
        public string? PreviousPriceDate { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("avg_cost")]
        public float? AvgCost { get; set; }
        [JsonPropertyName("market_value")]
        public float? MarketValue { get; set; }
        [JsonPropertyName("unrealized_pnl")]
        public float? UnrealizedPnl { get; set; }

        public bool HasQuote => Price is not null;

        /// <summary>
        /// What this position is counted at when there is no quote: its average cost. The
        /// portfolio roll-up needs a number to total, but it is not a market value and must
        /// never be presented as one.
        /// </summary>
        public float? CostBasisValue => AvgCost is null ? null : (float)Math.Round(AvgCost.Value * Quantity, 2);

        /// Value used where the charts need a complete picture of the book.
        public float ChartValue => (float)(MarketValue ?? CostBasisValue ?? 0f);

        /// <summary>
        /// Day-over-day change in unit price. Null unless both closes exist; comparing a
        /// session's close with itself is not a change.
        /// </summary>
        public float? Change {
            get {
                if (Price is null || PreviousPrice is null) return null;
                if (!string.IsNullOrWhiteSpace(PriceDate) && PriceDate == PreviousPriceDate) return 0f;
                return (float)Math.Round(Price.Value - PreviousPrice.Value, 2);
            }
        }
    }
}