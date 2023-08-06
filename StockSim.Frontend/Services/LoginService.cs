using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public class LoginService : ServiceBase, ILoginService {
        public LoginService(HttpClient http) : base(http) { }
        public async Task<TokenModel> Login(LoginModel user) {
            return await GetJsonAsync<TokenModel>(CreateUrl("/login"), user.Username, user.Password);
        }
    }
}
