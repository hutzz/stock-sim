using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class Dashboard {
        private UserModel user = new();
        private List<UserStockModel> userStocks = new();
        private string _message = "";
        private bool _toggleTradingMenu = false;
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
                StateHasChanged();
            }
            catch(Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/login");
            }
        }
        private async Task HandleBuy(BuySellDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.BuyStock(token, stock);
                await ShowMessage(response.Msg!);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message);
                Console.WriteLine(e);
            }
        }
        private async Task HandleTradingMenuBuy(TradingMenuDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.BuyStock(token, stock);
                await ShowMessage(response.Msg!);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message);
                Console.WriteLine(e);
            }
        }
        private async Task HandleSell(BuySellDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.SellStock(token, stock);
                await ShowMessage(response.Msg!);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message);
                Console.WriteLine(e);
            }
        }
        private async Task HandleTradingMenuSell(TradingMenuDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.SellStock(token, stock);
                await ShowMessage(response.Msg!);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message);
                Console.WriteLine(e);
            }
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