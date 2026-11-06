using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Search {
        private string searchTerm = "";
        private List<CompanyModel> stocks = new();
        // The universe is 10k+ tickers, so a short query can match thousands of names; the
        // dropdown shows the best matches and says how many more were left out.
        private const int MaxResults = 50;
        private int _hiddenResults;
        private List<CompanyModel> filteredStocks {
            get {
                if (string.IsNullOrWhiteSpace(searchTerm)) {
                    _hiddenResults = 0;
                    return stocks.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
                }
                var term = searchTerm.Trim().ToLower();
                // A ticker typed exactly is the match people want, so it leads the list.
                var matches = stocks
                    .Where(s => s.Name.ToLower().Contains(term) || s.Ticker.ToLower().Contains(term))
                    .OrderByDescending(s => s.Ticker.ToLower() == term)
                    .ThenBy(s => s.Name.Length)
                    .ThenBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                _hiddenResults = Math.Max(0, matches.Count - MaxResults);
                return _hiddenResults > 0 ? matches.Take(MaxResults).ToList() : matches;
            }
        }
        protected override async Task OnInitializedAsync() {
            base.OnInitialized();
            try {
                stocks = await SearchService.GetCompanies();
            }
            catch (Exception e) {
                // An empty list leaves the box useless for picking, but typing a ticker and
                // pressing Enter still opens it, which is what freeform search is for.
                Console.WriteLine(e);
                stocks = new List<CompanyModel>();
            }
            StateHasChanged();
        }
        private void SearchStock() {
            // An empty (or whitespace-only) query has no ticker to open, so the form does nothing.
            if (string.IsNullOrWhiteSpace(searchTerm)) {
                return;
            }
            // Relative URL keeps navigation in-app; an absolute URL forces a full page reload.
            NavigationManager.NavigateTo($"/stock/{searchTerm.Trim().ToUpper()}");
        }
        private void GoToStock(string ticker) {
            NavigationManager.NavigateTo($"/stock/{ticker.ToUpper()}");
        }
    }
}
