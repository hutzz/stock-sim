using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public interface IStockService {
        Task<StockModel> GetStockData(StockDto stockDto);
        Task<List<UserStockModel>> GetUserStockList(string token);
        Task<Message> BuyStock(string token, BuySellDto stock);
        Task<Message> BuyStock(string token, TradingMenuDto stock);
        Task<Message> SellStock(string token, BuySellDto stock);
        Task<Message> SellStock(string token, TradingMenuDto stock);
    }
}
