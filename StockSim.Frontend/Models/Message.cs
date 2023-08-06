using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class Message {
        [JsonPropertyName("message")]
        public string? Msg { get; set; }
    }
}
