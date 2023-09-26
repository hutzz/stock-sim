using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace StockSim.Frontend.Components {
    public partial class LineGraph {
        [Parameter]
        public List<float>? Data { get; set; }
        [Parameter]
        public List<string>? Labels { get; set; } 
        private async Task DrawGraph() {
            if (Data != null && Labels != null) {
                await JSRuntime.InvokeVoidAsync("drawLineGraph", Labels, Data);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                await DrawGraph();
            }
        }
        protected override async Task OnParametersSetAsync() {
            await JSRuntime.InvokeVoidAsync("updateLineGraph", Labels, Data);
        }
    }
}