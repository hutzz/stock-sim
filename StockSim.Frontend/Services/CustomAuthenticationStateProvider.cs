using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace StockSim.Frontend.Services {
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider {
        private readonly ITokenService _tokenService;
        public CustomAuthenticationStateProvider(ITokenService tokenService) {
            _tokenService = tokenService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            try {
                var token = await _tokenService.GetAccessTokenAsync();
                if (_tokenService.GetJwtExpiryTime(token) <= DateTime.UtcNow) {
                    await _tokenService.RefreshTokens();
                    token = await _tokenService.GetAccessTokenAsync();
                }
                var identity = (string.IsNullOrEmpty(token) || _tokenService.GetJwtExpiryTime(token) <= DateTime.Now) ? new ClaimsIdentity() : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                var user = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
            catch (Exception) {
                var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
        }
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt) {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
        private static byte[] ParseBase64WithoutPadding(string base64) {
            switch (base64.Length % 4) {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
