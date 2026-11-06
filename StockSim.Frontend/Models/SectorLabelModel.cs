using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    /// <summary>
    /// Industry label for one symbol from GET /stock/sectors. A null Sector means the
    /// provider has no quote for the symbol (delisted, renamed); charts show it as
    /// Unclassified rather than dropping the position.
    /// </summary>
    public class SectorLabelModel {
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [JsonPropertyName("sector")]
        public string? Sector { get; set; }
    }

    public class SectorLabelsResponse {
        [JsonPropertyName("sectors")]
        public List<SectorLabelModel> Sectors { get; set; } = new();
    }
}
