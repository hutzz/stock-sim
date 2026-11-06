using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StockSim.Frontend.Components;

/// <summary>
/// Radar of portfolio exposure per industry sector. The scale is relative to the
/// largest sector, so the shape shows concentration rather than absolute value.
/// </summary>
public partial class SectorRadar : ComponentBase
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    /// <summary>Sector names, one point on the radar.</summary>
    [Parameter] public string[] Labels { get; set; } = Array.Empty<string>();

    /// <summary>Share of portfolio value per sector, as percentages.</summary>
    [Parameter] public double[] Shares { get; set; } = Array.Empty<double>();

    /// <summary>Tickers held in each sector, shown in the tooltip.</summary>
    [Parameter] public string[][] Holdings { get; set; } = Array.Empty<string[]>();

    private bool drawn;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await JSRuntime.InvokeVoidAsync("drawSectorRadar", Labels, Shares, Holdings);
        drawn = true;
    }

    protected override async Task OnParametersSetAsync()
    {
        // Parameters can change before the canvas has been drawn for the first time.
        if (!drawn) return;
        await JSRuntime.InvokeVoidAsync("updateSectorRadar", Labels, Shares, Holdings);
    }
}
