using StockSim.Frontend.Models;

namespace StockSim.Frontend.Services {
    public interface ISearchService {
        /// <summary>All searchable companies: the SEC ticker universe with data.txt names where both list a symbol.</summary>
        Task<List<CompanyModel>> GetCompanies();
        /// <summary>Display name for a ticker, or an empty string when no source knows it.</summary>
        Task<string> GetNameByTicker(string ticker);
    }
}