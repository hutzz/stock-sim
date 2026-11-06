using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    /// <summary>
    /// The price a trade on one symbol fills at, from GET /stock/quote, together with the
    /// trading session that close belongs to. During an in-progress session the fill uses an
    /// earlier day's close, so the date is part of the quote rather than assumed to be today.
    /// </summary>
    public class QuoteModel {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [JsonPropertyName("price")]
        public float? Price { get; set; }
        [JsonPropertyName("price_date")]
        public string? PriceDate { get; set; }
    }
}
