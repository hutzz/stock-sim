using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Search {
        private string searchTerm = "";
        private List<CompanyModel> stocks = new();
        private List<CompanyModel> filteredStocks {
            get {
                if (string.IsNullOrWhiteSpace(searchTerm)) {
                    return stocks.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
                } else {
                    return stocks.Where(s => s.Name.ToLower().Contains(searchTerm.ToLower()) || s.Ticker.ToLower().Contains(searchTerm.ToLower())).OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
                }
            }
        }
        protected override async Task OnInitializedAsync() {
            base.OnInitialized();
            var response = await SearchService.GetCompanyList("data.txt");
            using (var reader = new StreamReader(response)) {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null) {
                    var items = line.Split(',');
                    stocks.Add(new CompanyModel { Ticker = items[0], Name = items[1] });
                }
            }
            StateHasChanged();
        }
        private void SearchStock() {
            NavigationManager.NavigateTo($"https://localhost:7003/stock/{searchTerm.ToUpper()}");
        }
    }
}
