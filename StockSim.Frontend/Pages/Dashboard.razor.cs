using Microsoft.JSInterop;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Dashboard {
        private UserModel user = new();
        private List<UserStockModel> userStocks = new();
        private PortfolioSummaryModel? summary;
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
                // The summary is additive detail; if it is unavailable the cards fall
                // back to the cash and holdings figures above rather than failing the page.
                try {
                    summary = await StockService.GetPortfolioSummary(token);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    summary = null;
                }
                _loading = false;
                StateHasChanged();
            }
            catch(Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/login");
            }
        }
        // Cash, holdings and combined totals shown in the portfolio summary cards. The
        // server-side summary is preferred; the client-side figures are the fallback.
        private float CashBalance => (float)Math.Round(summary?.Cash ?? user.Balance, 2);
        private float StocksValue => (float)Math.Round(summary?.MarketValue ?? userStocks.Sum(s => s.TotalValue), 2);
        private float TotalValue => (float)Math.Round(summary?.TotalValue ?? CashBalance + StocksValue, 2);
        // Profit already locked in by sales, versus profit still riding on open positions.
        private float RealizedPnl => (float)Math.Round(summary?.RealizedPnl ?? 0, 2);
        private float UnrealizedPnl => (float)Math.Round(summary?.UnrealizedPnl ?? 0, 2);
        private bool HasSummary => summary is not null;
        private static string PnlClass(float value) => value > 0 ? "pnl-up" : (value < 0 ? "pnl-down" : "pnl-flat");
        // Breakeven has no sign; a leading "+" on $0.00 reads as a gain that did not happen.
        private static string FormatSigned(float value) =>
            value > 0 ? $"+${value.ToString("F2")}" : (value < 0 ? $"-${Math.Abs(value).ToString("F2")}" : "$0.00");
        private void GoToStock(string symbol) {
            // In-app route change; the plain anchor's absolute href used to force a full reload.
            NavigationManager.NavigateTo($"/stock/{symbol}");
        }
        private async Task ShowMessage(string message) {
            _message = message;
            StateHasChanged();
            await Task.Delay(3000);
            message = "";
            StateHasChanged();
        }
    }
}