using StockSim.Frontend.Models;
using System.Collections.Generic;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;

namespace StockSim.Frontend.Services {
    public class TokenService : ServiceBase, ITokenService {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private const string _accessKey = "token";
        private const string _refreshKey = "refresh";
        public TokenService(HttpClient http, ILocalStorageService localStorage, NavigationManager navigationManager) : base(http) {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }
        public async Task StoreTokensAsync(TokenModel token) {
            await _localStorage.SetItemAsync(_accessKey, token.Access);
            await _localStorage.SetItemAsync(_refreshKey, token.Refresh);
        }
        public async Task<string> GetAccessTokenAsync() {
            return await _localStorage.GetItemAsync<string>(_accessKey);
        }
        public async Task<string> GetRefreshTokenAsync() {
            return await _localStorage.GetItemAsync<string>(_refreshKey);
        }
        public async Task DeleteTokensAsync() {
            await _localStorage.RemoveItemAsync(_accessKey);
            await _localStorage.RemoveItemAsync(_refreshKey);
        }
        public async Task RefreshTokens() {
            var refresh = await GetRefreshTokenAsync();
            var newTokens = await GetRefreshJsonAsync<TokenModel>(CreateUrl("/refresh"), refresh);
            await StoreTokensAsync(newTokens!);
        }
        public DateTime GetJwtExpiryTime(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo;
        }
        public async Task CheckToken() {
            try {
                var token = await GetAccessTokenAsync();
                if (GetJwtExpiryTime(token) <= DateTime.UtcNow) {
                    await RefreshTokens();
                    token = await GetAccessTokenAsync();
                } 
            }
            catch (Exception) {
                _navigationManager.NavigateTo("/login");
            }
        }
    }
}
