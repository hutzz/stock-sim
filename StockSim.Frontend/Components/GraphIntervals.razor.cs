using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Components {
    public partial class GraphIntervals {
        [Parameter]
        public EventCallback<StockDto> SelectInterval { get; set; }
    }
}