using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public class RegisterService : ServiceBase, IRegisterService {
        public RegisterService(HttpClient http) : base(http) { }
        public async Task<Message> Register(RegisterModel user) {
            return await PostJsonAsync<RegisterModel, Message>(CreateUrl("/register"), user);
        }
    }
}
