using Blazored.LocalStorage;
using StockSim.Frontend.Models;
using System.Collections.Generic;

namespace StockSim.Frontend.Services {
    public interface ITokenService {
        Task StoreTokensAsync(TokenModel token);
        Task<string> GetAccessTokenAsync();
        Task<string> GetRefreshTokenAsync();
        Task DeleteTokensAsync();
        Task RefreshTokens();
        DateTime GetJwtExpiryTime(string token);
        Task CheckToken();
    }
}
