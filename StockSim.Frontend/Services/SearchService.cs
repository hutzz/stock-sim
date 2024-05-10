namespace StockSim.Frontend.Services {
    public class SearchService : ServiceBase, ISearchService {
        public SearchService(HttpClient http) : base(http) {}
        public async Task<Stream> GetCompanyList(string path) {
            return await GetHttpClient().GetStreamAsync(path);
        }
    }
}