using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class Message {
        [JsonPropertyName("message")]
        public string? Msg { get; set; }
        // Trade responses report the price the order actually filled at and the session it
        // belongs to, so a confirmation can name both instead of implying today's price.
        [JsonPropertyName("filled_price")]
        public float? FilledPrice { get; set; }
        [JsonPropertyName("fill_date")]
        public string? FillDate { get; set; }
    }
}
