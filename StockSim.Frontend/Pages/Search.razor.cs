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
            stocks = await SearchService.BuildCompanyList("data.txt");
            StateHasChanged();
        }
        private void SearchStock() {
            // Relative URL keeps navigation in-app; an absolute URL forces a full page reload.
            NavigationManager.NavigateTo($"/stock/{searchTerm.ToUpper()}");
        }
        private void GoToStock(string ticker) {
            NavigationManager.NavigateTo($"/stock/{ticker.ToUpper()}");
        }
    }
}
