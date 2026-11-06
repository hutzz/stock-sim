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
        public async Task<PortfolioSummaryModel> GetPortfolioSummary(string token) {
            return await GetJsonAsync<PortfolioSummaryModel>(CreateUrl("/stock/summary"), token);
        }
        // Symbols are sent as one comma-separated list so a portfolio of any size costs a
        // single request; the server caps how many symbols one call may ask about.
        public async Task<List<SectorLabelModel>> GetSectorLabels(string token, IEnumerable<string> symbols) {
            var joined = string.Join(",", symbols);
            var response = await GetJsonAsync<SectorLabelsResponse>(CreateUrl($"/stock/sectors?symbols={joined}"), token);
            return response?.Sectors ?? new List<SectorLabelModel>();
        }
        public async Task<List<PortfolioHistoryPointModel>> GetPortfolioHistory(string token) {
            return await GetJsonAsync<List<PortfolioHistoryPointModel>>(CreateUrl("/stock/history"), token)
                ?? new List<PortfolioHistoryPointModel>();
        }
        // A quote is one lookup, so the trade forms fetch it as the typed symbol changes.
        public async Task<QuoteModel> GetQuote(string token, string symbol) {
            var url = CreateUrl($"/stock/quote?symbol={Uri.EscapeDataString(symbol)}");
            return await GetJsonAsync<QuoteModel>(url, token)
                ?? throw new Exception("No quote available for that stock.");
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
