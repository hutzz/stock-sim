using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Services;

namespace StockSim.Frontend.Components {
    /// <summary>
    /// Shows what an order would cost at the current fill quote before it is submitted.
    /// A fill always uses a settled session's close, so the estimate names that session next
    /// to the amount: during an in-progress session the number is an earlier day's close,
    /// not today's, and presenting it unlabelled reads as a live market price.
    /// </summary>
    public partial class TradeEstimate {
        [Parameter]
        public float? FillPrice { get; set; }
        // ISO date of the session the fill price came from, straight from the API.
        [Parameter]
        public string? FillDate { get; set; }
        [Parameter]
        public int Quantity { get; set; }
        [Parameter]
        public string Side { get; set; } = "Buy";

        private bool HasQuote => FillPrice is not null;
        private bool IsStale => HasQuote && !MarketClock.IsToday(FillDate);
        // "today's close" when current, "Nov 3 close" when the quote is from an older session.
        private string SessionLabel => MarketClock.QuoteAge(FillDate);
        private string QuantityText => $"{Quantity} {(Quantity == 1 ? "share" : "shares")}";
        private string FillTotalText => $"${Math.Round(FillPrice!.Value * Quantity, 2).ToString("F2")}";
    }
}
