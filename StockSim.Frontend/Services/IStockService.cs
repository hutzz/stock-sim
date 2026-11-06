using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public interface IStockService {
        Task<StockModel> GetStockData(StockDto stockDto);
        Task<List<UserStockModel>> GetUserStockList(string token);
        Task<PortfolioSummaryModel> GetPortfolioSummary(string token);
        Task<List<SectorLabelModel>> GetSectorLabels(string token, IEnumerable<string> symbols);
        Task<List<PortfolioHistoryPointModel>> GetPortfolioHistory(string token);
        // The price an order on this symbol fills at, tagged with its trading session.
        Task<QuoteModel> GetQuote(string token, string symbol);
        Task<Message> BuyStock(string token, BuySellDto stock);
        Task<Message> BuyStock(string token, TradingMenuDto stock);
        Task<Message> SellStock(string token, BuySellDto stock);
        Task<Message> SellStock(string token, TradingMenuDto stock);
    }
}
