using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Components {
    public partial class Sell {
        [Parameter]
        public string? Symbol { get; set; }
        // Current fill quote for this symbol, with the session it belongs to. Null when the
        // exchange has no usable close, which the estimate says outright.
        [Parameter]
        public float? FillPrice { get; set; }
        [Parameter]
        public string? FillDate { get; set; }
        [Parameter]
        public EventCallback<BuySellDto> OnSell { get; set; }
        [Parameter]
        public EventCallback<Message> OnFail { get; set; }
        private int _quantity { get; set; }
        public BuySellDto stock = new();
        // Explicit binding so typing a quantity re-renders the proceeds estimate under the field.
        private void OnQuantityChanged(int value) {
            _quantity = value;
            StateHasChanged();
        }
        protected override void OnInitialized() {
            Console.WriteLine(Symbol);
        }
        private async Task Valid() {
            stock = new() { Symbol = Symbol, Quantity = _quantity };
            await OnSell.InvokeAsync(stock);
            _quantity = 0;
            StateHasChanged();
        }
        private async Task Invalid() {
            await OnFail.InvokeAsync(new Message() { Msg = "An error occurred. Ensure that your input is a valid integer." });
        }
    }
}