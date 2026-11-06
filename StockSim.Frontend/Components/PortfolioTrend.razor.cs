using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StockSim.Frontend.Components;

/// <summary>Line of total portfolio value over the recorded daily snapshots.</summary>
public partial class PortfolioTrend : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    /// <summary>Snapshot dates in chronological order.</summary>
    [Parameter] public string[] Labels { get; set; } = Array.Empty<string>();

    /// <summary>Total portfolio value per snapshot, in dollars.</summary>
    [Parameter] public double[] Totals { get; set; } = Array.Empty<double>();

    private bool drawn;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await JSRuntime.InvokeVoidAsync("drawPortfolioTrend", Labels, Totals);
        drawn = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!drawn) return;
        await JSRuntime.InvokeVoidAsync("updatePortfolioTrend", Labels, Totals);
    }
}
