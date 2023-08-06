using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    public class UserModel {
        [JsonPropertyName("public_id")]
        public string? Id { get; set; }
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        [JsonPropertyName("balance")]
        public float Balance { get; set; }
    }
}