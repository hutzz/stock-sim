using System.Net.Http.Json;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Services {
    public class SearchService : ServiceBase, ISearchService {
        // One SEC row per listed ticker, plus the hand-cleaned names from data.txt where the
        // two agree on a listing. Cached for the life of the app: the universe file is
        // ~630KB, so it is read once per session rather than once per page visit.
        private static List<CompanyModel>? _companies;
        private static Dictionary<string, string>? _namesByTicker;

        public SearchService(HttpClient http) : base(http) {}

        public async Task<List<CompanyModel>> GetCompanies() {
            if (_companies is not null) return _companies;
            var universe = await LoadUniverse();
            var legacy = await LoadLegacyNames();
            var companies = new List<CompanyModel>(universe.Count + legacy.Count);
            foreach (var (ticker, name) in universe) {
                // data.txt carries hand-cleaned title-case names; SEC publishes raw EDGAR
                // strings, some ALL CAPS and some with jurisdictional noise ("ABM INDUSTRIES
                // INC /DE/"), so the curated spelling wins where both list the same symbol.
                var display = legacy.TryGetValue(ticker, out var curated) && !string.IsNullOrWhiteSpace(curated)
                    ? curated
                    : name;
                companies.Add(new CompanyModel { Ticker = ticker, Name = display });
            }
            // Symbols data.txt knows about that the SEC list does not stay searchable: the
            // app supports freeform tickers and Yahoo still quotes some of these shells.
            var listed = new HashSet<string>(universe.Keys, StringComparer.OrdinalIgnoreCase);
            foreach (var (ticker, name) in legacy) {
                if (!listed.Contains(ticker)) companies.Add(new CompanyModel { Ticker = ticker, Name = name });
            }
            _namesByTicker = companies.ToDictionary(c => c.Ticker, c => c.Name, StringComparer.OrdinalIgnoreCase);
            _companies = companies;
            return _companies;
        }

        // Freeform searching is deliberate, so an unknown ticker is not an error: callers get
        // an empty name and the stock page renders without a subtitle.
        public async Task<string> GetNameByTicker(string ticker) {
            if (string.IsNullOrWhiteSpace(ticker)) return "";
            await GetCompanies();
            return _namesByTicker!.TryGetValue(ticker.Trim(), out var name) ? name : "";
        }

        private async Task<Dictionary<string, string>> LoadUniverse() {
            try {
                var payload = await GetHttpClient().GetFromJsonAsync<Dictionary<string, UniverseEntryModel>>(UniversePath);
                var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var (ticker, entry) in payload ?? new Dictionary<string, UniverseEntryModel>()) {
                    if (!string.IsNullOrWhiteSpace(entry?.Name)) result[ticker] = entry.Name;
                }
                return result;
            }
            catch (Exception e) {
                Console.WriteLine($"universe.json unavailable, search falls back to data.txt only: {e}");
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private async Task<Dictionary<string, string>> LoadLegacyNames() {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try {
                using var response = await GetHttpClient().GetStreamAsync(LegacyListPath);
                using var reader = new StreamReader(response);
                string? line;
                while ((line = await reader.ReadLineAsync()) != null) {
                    // Split once: a company name can itself contain a comma.
                    var split = line.Split(',', 2);
                    if (split.Length < 2) continue;
                    var ticker = split[0].Trim();
                    var name = split[1].Trim();
                    if (ticker.Length > 0 && name.Length > 0) result.TryAdd(ticker, name);
                }
            }
            catch (Exception e) {
                Console.WriteLine($"data.txt unavailable: {e}");
            }
            return result;
        }

        private const string UniversePath = "universe.json";
        private const string LegacyListPath = "data.txt";
    }
}