using StockSim.Frontend.Models;

namespace StockSim.Frontend.Services {
    public class SearchService : ServiceBase, ISearchService {
        public SearchService(HttpClient http) : base(http) {}
        public async Task<Stream> GetCompanyList(string path) {
            return await GetHttpClient().GetStreamAsync(path);
        }
        public async Task<List<CompanyModel>> BuildCompanyList(string path) {
            var response = await GetCompanyList(path);
            List<CompanyModel> stocks = new();
            using (var reader = new StreamReader(response)) {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null) {
                    var items = line.Split(',');
                    stocks.Add(new CompanyModel { Ticker = items[0], Name = items[1] });
                }
            }
            return stocks;
        }
        public async Task<string> GetNameByTicker(string path, string ticker) {
            var stocks = await BuildCompanyList(path);
            return stocks.Where(s => s.Ticker.ToLower() == ticker.ToLower()).First().Name;
        }
    }
}