using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Components {
    public partial class Buy {
        [Parameter]
        public string? Symbol { get; set; }
        // Current fill quote for this symbol, with the session it belongs to. Null when the
        // exchange has no usable close, which the estimate says outright.
        [Parameter]
        public float? FillPrice { get; set; }
        [Parameter]
        public string? FillDate { get; set; }
        [Parameter]
        public EventCallback<BuySellDto> OnBuy { get; set; }
        [Parameter]
        public EventCallback<Message> OnFail { get; set; }
        private int _quantity { get; set; }
        public BuySellDto stock = new();
        protected override void OnInitialized() {
            Console.WriteLine(Symbol);
        }
        private async Task Valid() {
            stock = new() { Symbol = Symbol, Quantity = _quantity };
            await OnBuy.InvokeAsync(stock);
            _quantity = 0;
            StateHasChanged();
        }
        private async Task Invalid() {
            await OnFail.InvokeAsync(new Message() { Msg = "An error occurred. Ensure that your input is a valid integer." });
        }
    }
}
