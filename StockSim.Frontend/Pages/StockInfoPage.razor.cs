using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;
using StockSim.Frontend.Services;

namespace StockSim.Frontend.Pages {
    public partial class StockInfoPage {
        private int _userStockCount;
        private float _userStockValue;
        // Whether this position has a real quote, and the session that quote came from.
        // Without them "Worth $X" reads as a live number when it is a cost-basis figure.
        private bool _userHasQuote;
        private string? _userQuoteDate;
        // The price an order on this symbol fills at, shown on the trade forms before submit.
        private QuoteModel? _quote;
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
        // Hover text carries the full date (and time for intraday), which the axis labels leave out.
        public List<string>? GraphTooltipTitles => FormatTooltipTitles(Model?.DateTimes, _period);
        protected override async Task OnInitializedAsync() {
            try {
                await LoadPortfolioFacts();
                _name = await SearchService.GetNameByTicker(Symbol);
                StockDto stockDto = new() { Symbol=Symbol, Period=_period, Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
                await LoadFillQuote();
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
                        _userHasQuote = stockMatchingSymbol.HasQuote;
                        _userQuoteDate = stockMatchingSymbol.PriceDate;
                        // No quote means the position is carried at what it cost, which is not
                        // a market value and has to be labelled as such.
                        _userStockValue = (float)Math.Round(
                            stockMatchingSymbol.MarketValue ?? stockMatchingSymbol.CostBasisValue ?? 0f, 2);
                    }
                    else {
                        _userStockCount = 0;
                        _userHasQuote = false;
                        _userQuoteDate = null;
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
        // The price an order here fills at, shown on the trade forms before submitting. It is
        // the latest settled close, which during an in-progress session belongs to an earlier
        // day - so the form names that day instead of implying a live market price.
        private async Task LoadFillQuote() {
            _quote = null;
            if (string.IsNullOrWhiteSpace(Symbol)) return;
            try {
                var token = await TokenService.GetAccessTokenAsync();
                _quote = await StockService.GetQuote(token, Symbol!);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                _quote = null;
            }
        }
        private bool HasFillQuote => _quote?.Price is not null;
        // Fill price and its session passed to the trade forms, which quote each order
        // against them before it is submitted. A fill always uses a settled session's close,
        // so during an in-progress session the number is an earlier day's, not today's.
        private float? FillPrice => _quote?.Price;
        private string? FillDate => _quote?.PriceDate;
        // Explains what "Worth $X" on the ownership card is based on.
        private string OwnershipNote {
            get {
                if (_userStockCount == 0) return "You do not own any shares yet.";
                if (!_userHasQuote) return $"Worth {_userStockValue.ToString("F2")} at your average cost — no market quote.";
                var age = MarketClock.QuoteAge(_userQuoteDate);
                return age.Length == 0
                    ? $"Worth {_userStockValue.ToString("F2")}"
                    : $"Worth {_userStockValue.ToString("F2")} — {age}";
            }
        }
        // Confirmation text names the price the order filled at and its session, so a fill
        // during an in-progress session is not read as happening at today's price.
        private static string FillSummary(Message response, string fallback) {
            if (response.FilledPrice is null) return fallback;
            var price = $"${Math.Round(response.FilledPrice.Value, 2).ToString("F2")}";
            var age = MarketClock.QuoteAge(response.FillDate);
            return age.Length == 0
                ? $"{fallback} Filled at {price}."
                : $"{fallback} Filled at {price}, {age}.";
        }
        private async Task HandleBuy(BuySellDto stock) {
            await TokenService.CheckToken();
            try {
                var token = await TokenService.GetAccessTokenAsync();
                var response = await StockService.BuyStock(token, stock);
                await LoadPortfolioFacts();
                await RefreshGraph("1d", "1h");
                await LoadFillQuote();
                await ShowMessage(FillSummary(response, response.Msg ?? "Order placed."), isSuccess: true);
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
                await LoadFillQuote();
                await ShowMessage(FillSummary(response, response.Msg ?? "Order placed."), isSuccess: true);
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
        // The yearly view is weekly data, so it shows the day of the month too - a bare "Jul 26"
        // repeats four times and gives no clue which week of July a point sits on.
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
                    "1y" => stamp.ToString("MMM d"),
                    _ => stamp.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return formatted;
        }
        // Axis labels are abbreviated, so the hover tooltip spells out the full date (plus the
        // time for intraday points) to pin each point to an exact moment.
        private static List<string>? FormatTooltipTitles(List<string>? rawLabels, string period) {
            if (rawLabels == null) return null;
            var titles = new List<string>(rawLabels.Count);
            foreach (var raw in rawLabels) {
                if (!DateTime.TryParse(raw, out DateTime stamp)) {
                    titles.Add(raw);
                    continue;
                }
                titles.Add(period == "1d" ? stamp.ToString("MMM d, yyyy HH:mm") : stamp.ToString("MMM d, yyyy"));
            }
            return titles;
        }
    }
}
