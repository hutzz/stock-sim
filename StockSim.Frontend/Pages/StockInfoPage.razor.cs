using Microsoft.AspNetCore.Components;
using StockSim.Frontend.Models;

namespace StockSim.Frontend.Pages {
    public partial class StockInfoPage {
        [Parameter]
        public string? Symbol { get; set; }
        public StockModel? Model { get; set; }
        protected override async Task OnInitializedAsync() {
            try {
                StockDto stockDto = new() { Symbol=Symbol, Period="1d", Interval="1h" };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
                Console.WriteLine(Model.DateTimes);
                Console.WriteLine(Model.DateTimes?[0]);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("https://localhost:7003/dashboard");
            }
        }
        public async Task OnIntervalChange(StockDto dto) {
            try {
                StockDto stockDto = new() { Symbol=Symbol, Period=dto.Period, Interval=dto.Interval };
                Model = await StockService.GetStockData(stockDto);
                StateHasChanged();
                Console.WriteLine(Model.Close);
                Console.WriteLine(Model?.DateTimes?[0]);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                NavigationManager.NavigateTo("https://localhost:7003/dashboard");
            }
        }
    }
}