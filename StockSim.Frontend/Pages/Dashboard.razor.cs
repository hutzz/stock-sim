using Microsoft.JSInterop;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Dashboard {
        private UserModel user = new();
        private List<UserStockModel> userStocks = new();
        private string _message = "";
        private bool _toggleTradingMenu = false;
        private bool _loading = true;
        protected override async Task OnInitializedAsync() {
            base.OnInitialized();
            StateHasChanged();
            await LoadUserData();
        }
        private async Task LoadUserData() {
            try {
                var token = await TokenService.GetAccessTokenAsync();
                if (TokenService.GetJwtExpiryTime(token) <= DateTime.UtcNow) {
                    await TokenService.RefreshTokens();
                    token = await TokenService.GetAccessTokenAsync();
                }
                user = await UserService.GetUserData(token);
                try {
                    userStocks = await StockService.GetUserStockList(token);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    userStocks = new List<UserStockModel>();
                }
                _loading = false;
                StateHasChanged();
            }
            catch(Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/login");
            }
        }
        // Cash, holdings and combined totals shown in the portfolio summary cards.
        private float CashBalance => (float)Math.Round(user.Balance, 2);
        private float StocksValue => (float)Math.Round(userStocks.Sum(s => s.TotalValue), 2);
        private float TotalValue => (float)Math.Round(CashBalance + StocksValue, 2);
        private async Task ShowMessage(string message) {
            _message = message;
            StateHasChanged();
            await Task.Delay(3000);
            message = "";
            StateHasChanged();
        }
    }
}