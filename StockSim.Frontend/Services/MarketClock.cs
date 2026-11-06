using System.Globalization;

namespace StockSim.Frontend.Services {
    /// <summary>
    /// The API reports prices with the trading session they came from, and Yahoo publishes
    /// no settled close for an in-progress session, weekends or holidays - so the freshest
    /// close is routinely from an earlier day. These helpers turn those session dates into
    /// the labels that sit next to a price, which is what keeps an older close from being
    /// read as today's.
    /// </summary>
    public static class MarketClock {
        private static readonly CultureInfo MarketCulture = new("en-US");

        /// True when the session date is the local calendar day, i.e. nothing is stale.
        public static bool IsToday(string? sessionDate) =>
            DateOnly.TryParse(sessionDate, out var date) && date == DateOnly.FromDateTime(DateTime.Today);

        /// Short session label, e.g. "Nov 3"; empty when the API gave us no date.
        public static string Format(string? sessionDate) =>
            DateOnly.TryParse(sessionDate, out var date) ? date.ToString("MMM d", MarketCulture) : "";

        /// Session label naming it as a market close, e.g. "Nov 3 close".
        public static string CloseLabel(string? sessionDate) {
            var label = Format(sessionDate);
            return label.Length == 0 ? "" : $"{label} close";
        }

        /// Says whether a quote is current or from an earlier session: "today's close"
        /// versus "Nov 3 close". Empty when we have no date to judge the quote by.
        public static string QuoteAge(string? sessionDate) {
            if (string.IsNullOrWhiteSpace(sessionDate)) return "";
            return IsToday(sessionDate) ? "today's close" : CloseLabel(sessionDate);
        }
    }
}
