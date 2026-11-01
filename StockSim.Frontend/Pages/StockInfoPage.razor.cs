using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class StockInfoPage {
        private int _userStockCount;
        private float _userStockValue;
        private float _cashBalance;
        private string _message = "";
        private bool _messageIsSuccess;
        private string _name = "";
        private string _period = "1d";
        private CancellationTokenSource? _messageCts;
        [Parameter]
        public string? Symbol { get; set; }
        public StockModel? Model { get; set; }
        public float CashBalance => _cashBalance;
        public float UserStockValue => _userStockValue;
        // X-axis labels are pre-formatted here so the chart never shows raw timestamps with seconds.
        public List<string>? GraphLabels => FormatLabels(Model?.DateTimes, _period);
        protected override async Task OnInitializedAsync() {
            try {
                await LoadPortfolioFacts();
                _name = await SearchService.GetNameByTicker("data.txt", Symbol);
                StockDto stockDto = new() { Symbol=Symbol, Period=_period, Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/stock/notfound");
            }
        }
        // Quantity/value of this ticker plus the account cash balance shown in the page stats.
        private async Task LoadPortfolioFacts() {
            try {
                var token = await TokenService.GetAccessTokenAsync();
                if (TokenService.GetJwtExpiryTime(token) <= DateTime.UtcNow) {
                    await TokenService.RefreshTokens();
                    token = await TokenService.GetAccessTokenAsync();
                }
                try {
                    List<UserStockModel> userStocks = await StockService.GetUserStockList(token);
                    UserStockModel? stockMatchingSymbol = userStocks.FirstOrDefault(sym => sym?.Symbol?.ToLower() == Symbol?.ToLower());
                    if (stockMatchingSymbol != null) {
                        _userStockCount = stockMatchingSymbol.Quantity;
                        _userStockValue = stockMatchingSymbol.TotalValue;
                    }
                    else {
                        _userStockCount = 0;
                        _userStockValue = 0f;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                try {
                    UserModel user = await UserService.GetUserData(token);
                    _cashBalance = (float)Math.Round(user.Balance, 2);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/login");
            }
        }
        public async Task OnIntervalChange(StockDto dto) {
            try {
                _period = dto.Period ?? "1d";
                StockDto stockDto = new() { Symbol=Symbol, Period=dto.Period, Interval=dto.Interval };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/dashboard");
            }
        }
        private async Task HandleBuy(BuySellDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.BuyStock(token, stock);
                await LoadPortfolioFacts();
                await RefreshGraph("1d", "1h");
                await ShowMessage(response.Msg!, isSuccess: true);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message, isSuccess: false);
                Console.WriteLine(e);
            }
        }
        private async Task HandleSell(BuySellDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.SellStock(token, stock);
                await LoadPortfolioFacts();
                await RefreshGraph("1d", "1h");
                await ShowMessage(response.Msg!, isSuccess: true);
                StateHasChanged();
            }
            catch (Exception e) {
                await ShowMessage(e.Message, isSuccess: false);
                Console.WriteLine(e);
            }
        }
        private async Task HandleFail(Message error) {
            await ShowMessage(error.Msg!, isSuccess: false);
        }
        private async Task RefreshGraph(string period, string interval) {
            _period = period;
            StockDto stockDto = new() { Symbol=Symbol, Period=period, Interval=interval };
            Model = await StockService.GetStockData(stockDto);
        }
        private async Task ShowMessage(string message, bool isSuccess) {
            // Cancel any in-flight auto-dismiss so a newer banner is not wiped by the older timer.
            _messageCts?.Cancel();
            var cts = new CancellationTokenSource();
            _messageCts = cts;
            _message = message;
            _messageIsSuccess = isSuccess;
            StateHasChanged();
            try {
                await Task.Delay(3500, cts.Token);
            }
            catch (TaskCanceledException) {
                return;
            }
            if (_messageCts == cts) {
                _message = "";
                StateHasChanged();
            }
        }
        // "Today" keeps hours and minutes but drops seconds; longer ranges drop the time entirely.
        private static List<string>? FormatLabels(List<string>? rawLabels, string period) {
            if (rawLabels == null) return null;
            var formatted = new List<string>(rawLabels.Count);
            foreach (var raw in rawLabels) {
                if (!DateTime.TryParse(raw, out DateTime stamp)) {
                    formatted.Add(raw);
                    continue;
                }
                formatted.Add(period switch {
                    "1d" => stamp.ToString("HH:mm"),
                    "7d" => stamp.ToString("MMM d"),
                    "1mo" => stamp.ToString("MMM d"),
                    "1y" => stamp.ToString("MMM yy"),
                    _ => stamp.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return formatted;
        }
    }
}
