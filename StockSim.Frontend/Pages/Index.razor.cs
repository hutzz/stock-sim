using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Index {
        private void RedirectToLogin() {
            NavigationManager.NavigateTo("/login");
        }
        private void RedirectToDashboard() {
            NavigationManager.NavigateTo("/dashboard");
        }
    }
}