using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public interface IUserService {
        Task<UserModel> GetUserData(string token);
    }
}
