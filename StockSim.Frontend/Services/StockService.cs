using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public class StockService : ServiceBase, IStockService {
        public StockService(HttpClient http) : base(http) { }
        public async Task<StockModel> GetStockData(StockDto stockDto) {
            return await PostJsonAsync<StockDto, StockModel>(CreateUrl($"/stock/{stockDto.Symbol}"), stockDto);
        }
        public async Task<List<UserStockModel>> GetUserStockList(string token) {
            return await GetJsonAsync<List<UserStockModel>>(CreateUrl($"/stock/get"), token);
        }
        public async Task<Message> BuyStock(string token, BuySellDto stock) {
            return await PostJsonAsync<BuySellDto, Message>(CreateUrl("/stock/buy"), token, stock);
        }
        public async Task<Message> BuyStock(string token, TradingMenuDto stock) {
            return await PostJsonAsync<TradingMenuDto, Message>(CreateUrl("/stock/buy"), token, stock);
        }
        public async Task<Message> SellStock(string token, BuySellDto stock) {
            return await PostJsonAsync<BuySellDto, Message>(CreateUrl("/stock/sell"), token, stock);
        }
        public async Task<Message> SellStock(string token, TradingMenuDto stock) {
            return await PostJsonAsync<TradingMenuDto, Message>(CreateUrl("/stock/sell"), token, stock);
        }
    }
}
