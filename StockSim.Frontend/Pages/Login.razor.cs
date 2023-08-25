using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Login {
        private LoginModel user = new();
        private string message = "";

        private async Task Valid() {
            try {
                var result = await LoginService.Login(user);
                await TokenService.StoreTokensAsync(result);
                StateHasChanged();
                NavigationManager.NavigateTo("/dashboard", true);
            }
            catch (Exception e) {
                message = e.Message;
            }
        }
        private void Invalid() {
            message = "fuck";
            StateHasChanged();
        }
        private void RedirectToDashboard() {
            NavigationManager.NavigateTo("/dashboard");
        }
    }
}