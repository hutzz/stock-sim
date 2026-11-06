using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StockSim.Frontend.Components;

/// <summary>
/// Concentration doughnut: one slice per position, ranked by market value. Slices past
/// the visible limit collapse into "Other" so the ring stays readable on a large book.
/// </summary>
public partial class StockPie : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    /// <summary>Tickers, biggest first, with "Other" last when present.</summary>
    [Parameter] public string[] Labels { get; set; } = Array.Empty<string>();

    /// <summary>Market value per slice, in dollars.</summary>
    [Parameter] public double[] Values { get; set; } = Array.Empty<double>();

    /// <summary>Sector name per slice, shown in the tooltip.</summary>
    [Parameter] public string[] Sectors { get; set; } = Array.Empty<string>();

    private bool drawn;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await JSRuntime.InvokeVoidAsync("drawStockPie", Labels, Values, Sectors);
        drawn = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        // Parameters can change before the canvas has been drawn for the first time.
        if (!drawn) return;
        await JSRuntime.InvokeVoidAsync("updateStockPie", Labels, Values, Sectors);
    }
}
