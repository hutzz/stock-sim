using System.Text.Json.Serialization;

namespace StockSim.Frontend.Models {
    /// <summary>
    /// One row of wwwroot/universe.json, the SEC EDGAR ticker list that backs search.
    /// Built by tools/build_universe.py in the API repo and checked in as a static asset.
    /// </summary>
    public class UniverseEntryModel {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("exchange")]
        public string? Exchange { get; set; }
    }
}
