using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public interface IRegisterService {
        Task<Message> Register(RegisterModel user);
    }
}
