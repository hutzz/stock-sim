using Microsoft.JSInterop;
using StockSim.Frontend.Models;
using StockSim.Frontend.Services;

namespace StockSim.Frontend.Pages {
    public partial class Dashboard {
        private UserModel user = new();
        private List<UserStockModel> userStocks = new();
        private PortfolioSummaryModel? summary;
        private string _message = "";
        private bool _toggleTradingMenu = false;
        private bool _loading = true;
        // Charts are additive detail like the summary: if the sector labels or the history
        // are unavailable the panel shows a placeholder instead of failing the page.
        private string[] sectorNames = Array.Empty<string>();
        private double[] sectorShares = Array.Empty<double>();
        private string[][] sectorHoldings = Array.Empty<string[]>();
        private string[] stockLabels = Array.Empty<string>();
        private double[] stockValues = Array.Empty<double>();
        private string[] stockSectors = Array.Empty<string>();
        private string[] trendLabels = Array.Empty<string>();
        private double[] trendTotals = Array.Empty<double>();
        // Bucket for holdings Yahoo reports no sector for (delisted, renamed, exotic share class).
        private const string UnclassifiedLabel = "Unclassified";
        // Slice that collects the positions too small to show individually on the doughnut.
        private const string OtherLabel = "Other";
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
                await LoadChartData(token);
                _loading = false;
                StateHasChanged();
            }
            catch(Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("/login");
            }
        }
        // Sector labels and snapshot history feed the charts panel. Both are optional:
        // a failure leaves the matching chart on its placeholder.
        private async Task LoadChartData(string token) {
            sectorNames = Array.Empty<string>();
            sectorShares = Array.Empty<double>();
            sectorHoldings = Array.Empty<string[]>();
            stockLabels = Array.Empty<string>();
            stockValues = Array.Empty<double>();
            stockSectors = Array.Empty<string>();
            trendLabels = Array.Empty<string>();
            trendTotals = Array.Empty<double>();
            var symbols = userStocks.Select(s => s.Symbol).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Distinct().ToList();
            if (symbols.Count > 0) {
                try {
                    var labels = await StockService.GetSectorLabels(token, symbols);
                    BuildChartAggregates(labels);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
            try {
                var history = await StockService.GetPortfolioHistory(token);
                trendLabels = history.Where(p => !string.IsNullOrWhiteSpace(p.Date)).Select(p => p.Date!).ToArray();
                trendTotals = history.Where(p => !string.IsNullOrWhiteSpace(p.Date)).Select(p => Math.Round(p.TotalValue, 2)).ToArray();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        // Two views of the same holdings: the radar groups market value by industry, the
        // doughnut maps it position by position. Holdings Yahoo has no label for land in an
        // explicit "Unclassified" bucket so the charts stay honest about missing data.
        private void BuildChartAggregates(List<SectorLabelModel> labels) {
            var sectorBySymbol = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var label in labels) {
                if (!string.IsNullOrWhiteSpace(label.Symbol)) sectorBySymbol[label.Symbol] = label.Sector;
            }
            string SectorOf(UserStockModel holding) =>
                sectorBySymbol.TryGetValue(holding.Symbol ?? "", out var found) && !string.IsNullOrWhiteSpace(found)
                    ? found
                    : UnclassifiedLabel;
            var total = userStocks.Sum(s => s.ChartValue);
            // A zero-value portfolio has no shares to plot; keep the panel on its placeholders.
            if (total <= 0) return;
            var grouped = userStocks
                .GroupBy(SectorOf)
                .OrderByDescending(g => g.Sum(s => s.ChartValue))
                .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
                .ToList();
            sectorNames = grouped.Select(g => g.Key).ToArray();
            sectorShares = grouped.Select(g => Math.Round(g.Sum(s => s.ChartValue) / total * 100, 1)).ToArray();
            sectorHoldings = grouped.Select(g => g.Select(s => s.Symbol ?? "").Distinct().ToArray()).ToArray();
            BuildConcentrationSlices(SectorOf);
        }
        // Ranked positions, with everything past the visible limit folded into one "Other"
        // slice so a large book stays readable. The tail is only worth a slice when it
        // actually holds money.
        private const int MaxVisiblePositions = 8;
        private void BuildConcentrationSlices(Func<UserStockModel, string> sectorOf) {
            var ranked = userStocks
                .Where(s => !string.IsNullOrWhiteSpace(s.Symbol))
                .OrderByDescending(s => s.ChartValue)
                .ToList();
            if (ranked.Count == 0) return;
            var shown = ranked.Take(MaxVisiblePositions).ToList();
            var rest = ranked.Skip(MaxVisiblePositions).ToList();
            var restValue = Math.Round(rest.Sum(s => s.ChartValue), 2);
            if (restValue <= 0) rest = new List<UserStockModel>();
            stockLabels = shown.Select(s => s.Symbol!).Concat(rest.Count > 0 ? new[] { OtherLabel } : Array.Empty<string>()).ToArray();
            stockValues = shown.Select(s => Math.Round(s.ChartValue, 2)).Concat(rest.Count > 0 ? new[] { restValue } : Array.Empty<double>()).ToArray();
            stockSectors = shown.Select(sectorOf).Concat(rest.Count > 0 ? new[] { $"{rest.Count} more {(rest.Count == 1 ? "position" : "positions")}" } : Array.Empty<string>()).ToArray();
        }
        // Cash, holdings and combined totals shown in the portfolio summary cards. The
        // server-side summary is preferred; the client-side figures are the fallback.
        private float CashBalance => (float)Math.Round(summary?.Cash ?? user.Balance, 2);
        private float StocksValue => (float)Math.Round(summary?.MarketValue ?? userStocks.Sum(s => s.ChartValue), 2);
        private float TotalValue => (float)Math.Round(summary?.TotalValue ?? CashBalance + StocksValue, 2);
        // Profit already locked in by sales, versus profit still riding on open positions.
        private float RealizedPnl => (float)Math.Round(summary?.RealizedPnl ?? 0, 2);
        private float UnrealizedPnl => (float)Math.Round(summary?.UnrealizedPnl ?? 0, 2);
        private bool HasSummary => summary is not null;
        // Holdings priced at their average cost because Yahoo has no close for them. Their
        // share of "Value of stocks owned" is carried at cost, so the card says so.
        private List<string> UnquotedSymbols => summary?.UnquotedSymbols ?? new List<string>();
        private bool HasCostBasisHoldings => UnquotedSymbols.Count > 0;
        private string CostBasisNote {
            get {
                var symbols = UnquotedSymbols.Take(3);
                var listed = string.Join(", ", symbols);
                var extra = UnquotedSymbols.Count - symbols.Count();
                if (extra > 0) listed += $" +{extra} more";
                return $"{listed} {(UnquotedSymbols.Count == 1 ? "is" : "are")} priced at your average cost - no market quote.";
            }
        }
        // The session every price-derived figure on this page comes from. Naming it is what
        // stops a Friday close shown on Saturday from reading as today's number.
        private string QuoteDateLabel => MarketClock.Format(summary?.QuoteDate);
        private bool HasQuoteDate => !string.IsNullOrWhiteSpace(QuoteDateLabel);
        private bool QuoteIsStale => HasQuoteDate && !MarketClock.IsToday(summary!.QuoteDate);
        // Says in words how current the quotes behind the cards are.
        private string AsOfText => HasQuoteDate
            ? (QuoteIsStale ? $"Prices as of the {QuoteDateLabel}" : "Prices as of today's close")
            : "No market quote available";
        private string SummaryMeta {
            get {
                if (!HasSummary) return "From your cash and holdings";
                return HasCostBasisHoldings ? $"{AsOfText}; some holdings at cost" : AsOfText;
            }
        }
        private bool HasSectorCharts => sectorNames.Length > 0;
        private bool HasPositionCharts => stockLabels.Length > 0;
        // Says plainly whether the doughnut shows every position or a fold of the smallest ones.
        private string PieSubtitle {
            get {
                var positions = userStocks.Count(s => s.ChartValue > 0);
                if (positions == 0) return "Market value per position";
                var shown = Math.Min(positions, MaxVisiblePositions);
                return shown < positions
                    ? $"Top {shown} of {positions} positions by value"
                    : $"{positions} {(positions == 1 ? "position" : "positions")} by value";
            }
        }
        private bool HasTrendChart => trendTotals.Length > 0;
        // History only accumulates when a dashboard load records a snapshot, so it starts as one point.
        private string TrendMeta => trendTotals.Length switch {
            0 => "No history yet",
            1 => "History starts today",
            _ => $"{trendTotals.Length} daily points"
        };
        private static string PnlClass(float value) => value > 0 ? "pnl-up" : (value < 0 ? "pnl-down" : "pnl-flat");
        // Breakeven has no sign; a leading "+" on $0.00 reads as a gain that did not happen.
        private static string FormatSigned(float value) =>
            value > 0 ? $"+${value.ToString("F2")}" : (value < 0 ? $"-${Math.Abs(value).ToString("F2")}" : "$0.00");
        // Holdings with no quote are shown at their average cost - the same fallback the
        // roll-up uses - but the tile labels them as such instead of showing a market value.
        private static float TileValue(UserStockModel stock) =>
            (float)Math.Round(stock.MarketValue ?? stock.CostBasisValue ?? 0f, 2);
        // Names the session behind a tile's price so an older close is never read as today's.
        private static string TileAsOf(UserStockModel stock) {
            var age = MarketClock.QuoteAge(stock.PriceDate);
            return age.Length == 0 ? "No session date from the exchange" : $"{age}";
        }
        private static string TileTrendTitle(UserStockModel stock) {
            if (stock.Change is null) return "No previous close to compare with";
            var from = MarketClock.Format(stock.PreviousPriceDate);
            var to = MarketClock.Format(stock.PriceDate);
            if (from.Length == 0 || to.Length == 0) return "Change in unit price since the previous close";
            return from == to
                ? $"No change within the {to} session"
                : $"{from} close to {to} close";
        }
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