using StockSim.Frontend.Models;

namespace StockSim.Frontend.Services {
    public interface ISearchService {
        Task<Stream> GetCompanyList(string path);
        Task<List<CompanyModel>> BuildCompanyList(string path);
        Task<string> GetNameByTicker(string path, string ticker);
    }
}