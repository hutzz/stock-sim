using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Components {
    public partial class GraphIntervals {
        public record IntervalOption(string Label, string Period, string Interval);

        // Single source of truth for the ranges; the page reads its default from here.
        public static readonly IntervalOption[] Intervals = new[]
        {
            new IntervalOption("Today", "1d", "1h"),
            new IntervalOption("7d", "7d", "1d"),
            new IntervalOption("Month", "1mo", "1d"),
            new IntervalOption("Year", "1y", "1wk"),
        };

        [Parameter]
        public string? Active { get; set; }
        [Parameter]
        public EventCallback<StockDto> SelectInterval { get; set; }

        private async Task Select(IntervalOption option) {
            await SelectInterval.InvokeAsync(new StockDto() { Period = option.Period, Interval = option.Interval });
        }
    }
}