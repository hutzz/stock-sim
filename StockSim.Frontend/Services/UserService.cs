using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public class UserService : ServiceBase, IUserService {
        public UserService(HttpClient http) : base(http) { }
        public async Task<UserModel> GetUserData(string token) {
            return await GetJsonAsync<UserModel>(CreateUrl("/me"), token);
        }
    }
}
