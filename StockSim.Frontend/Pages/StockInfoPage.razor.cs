using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class StockInfoPage {
        private int _userStockCount;
        private string _message = "";
        [Parameter]
        public string? Symbol { get; set; }
        public StockModel? Model { get; set; }
        protected override async Task OnInitializedAsync() {
            try {
                _userStockCount = await UserStockCount(Symbol);
                StockDto stockDto = new() { Symbol=Symbol, Period="1d", Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("https://localhost:7003/dashboard");
            }
        }
        private async Task<int> UserStockCount(string symbol) {
            try {
                var token = await TokenService.GetAccessTokenAsync();
                if (TokenService.GetJwtExpiryTime(token) <= DateTime.UtcNow) {
                    await TokenService.RefreshTokens();
                    token = await TokenService.GetAccessTokenAsync();
                }
                try {
                    List<UserStockModel> userStocks = await StockService.GetUserStockList(token);
                    UserStockModel? stockMatchingSymbol = userStocks.FirstOrDefault(sym => sym?.Symbol?.ToLower() == symbol.ToLower());
                    if (stockMatchingSymbol != null)
                        return stockMatchingSymbol.Quantity;
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/login");
            }
            return 0;
        }
        public async Task OnIntervalChange(StockDto dto) {
            try {
                StockDto stockDto = new() { Symbol=Symbol, Period=dto.Period, Interval=dto.Interval };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("https://localhost:7003/dashboard");
            }
        }
        private async Task HandleBuy(BuySellDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.BuyStock(token, stock);
                _userStockCount = await UserStockCount(Symbol);
                StockDto stockDto = new() { Symbol=Symbol, Period="1d", Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
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
                _userStockCount = await UserStockCount(Symbol);
                StockDto stockDto = new() { Symbol=Symbol, Period="1d", Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
                await ShowMessage(response.Msg!);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message);
                Console.WriteLine(e);
            }
        }
        private async Task HandleFail(Message error) {
            await ShowMessage(error.Msg!);
        }
        private async Task ShowMessage(string message) {
            _message = message;
            StateHasChanged();
            await Task.Delay(3000);
            _message = "";
            StateHasChanged();
        }
    }
}