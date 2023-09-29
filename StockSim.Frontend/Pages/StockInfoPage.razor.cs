using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class StockInfoPage {
        private int _userStockCount;
        [Parameter]
        public string? Symbol { get; set; }
        public StockModel? Model { get; set; }
        protected override async Task OnInitializedAsync() {
            try {
                _userStockCount = await UserStockCount(Symbol);
                StockDto stockDto = new() { Symbol=Symbol, Period="1d", Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
                Console.WriteLine(Model.DateTimes);
                Console.WriteLine(Model.DateTimes?[0]);
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
                Console.WriteLine(Model.Close);
                Console.WriteLine(Model?.DateTimes?[0]);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("https://localhost:7003/dashboard");
            }
        }
    }
}