using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public interface ILoginService {
        Task<TokenModel> Login(LoginModel user);
    }
}
