namespace StockSim.Frontend.Services {
    public interface ISearchService {
        Task<Stream> GetCompanyList(string path);
    }
}