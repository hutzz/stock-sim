using System.Net;
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;
using StockSim.Frontend.Models;
using System.Net.Http.Headers;

namespace StockSim.Frontend.Services
{
    public class ServiceBase {
        private readonly HttpClient _httpClient;
        protected ServiceBase(HttpClient httpClient) {
            _httpClient = httpClient;
        }
        protected HttpClient GetHttpClient() {
            return _httpClient;
        }
        public string CreateUrl(string route) {
            string url = "http://127.0.0.1:5000";
            return url + route;
        }
        // no auth
        protected async Task<T?> GetJsonAsync<T>(string url) {
            var response = await GetHttpClient().GetAsync(url);
            if (CheckResponse(response) && ValidateJsonContent(response.Content)) {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            var msg = GetMessage(response);
            throw new Exception($"{msg}");
        }
        // basic auth
        protected async Task<T?> GetJsonAsync<T>(string url, string username, string password) {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            var response = await GetHttpClient().SendAsync(request);
            if (CheckResponse(response) && ValidateJsonContent(response.Content)) {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            var msg = GetMessage(response);
            throw new Exception($"{msg}");
        }
        // jwt auth
        protected async Task<T?> GetJsonAsync<T>(string url, string token) {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-access-token", token);
            var response = await GetHttpClient().SendAsync(request);
            if (CheckResponse(response) && ValidateJsonContent(response.Content)) {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            var msg = GetMessage(response);
            throw new Exception($"{msg}");
        }
        protected async Task<T?> GetRefreshJsonAsync<T>(string url, string token) {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("x-refresh-token", token);
            var response = await GetHttpClient().SendAsync(request);
            if (CheckResponse(response) && ValidateJsonContent(response.Content)) {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            var msg = GetMessage(response);
            throw new Exception($"{msg}");
        }
        protected async Task<TResult?> PostJsonAsync<TValue, TResult>(string url, TValue body) {
            var response = await GetHttpClient().PostAsJsonAsync(url, body);
            if (CheckResponse(response) && ValidateJsonContent(response.Content)) {
                var result = await response.Content.ReadFromJsonAsync<TResult>();
                return result;
            }
            var msg = GetMessage(response);
            throw new Exception($"{msg}");
        }
        protected async Task<TResult?> PostJsonAsync<TValue, TResult>(string url, string token, TValue body) {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("x-access-token", token);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await GetHttpClient().SendAsync(request);
            if (CheckResponse(response) && ValidateJsonContent(response.Content)) {
                var result = await response.Content.ReadFromJsonAsync<TResult>();
                return result;
            }
            var msg = GetMessage(response);
            throw new Exception($"{msg}");
        }
        protected async Task<T?> PostJsonAsync<T>(string url, T body) {
            return await PostJsonAsync<T, T>(url, body);
        }
        private static bool ValidateJsonContent(HttpContent content) {
            var mediaType = content?.Headers.ContentType?.MediaType;
            return mediaType != null && mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase);
        }
        private bool CheckResponse(HttpResponseMessage response) {
            if (response.IsSuccessStatusCode) return true;
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.NotFound) {
                Console.WriteLine($"Request: {response.RequestMessage?.RequestUri}");
                Console.WriteLine($"Response status: {response.StatusCode} {response.ReasonPhrase}");
            }
            return false;
        }
        private string GetMessage(HttpResponseMessage response) {
            var message = JsonSerializer.Deserialize<Message>(response.Content.ReadAsStream());
            return message!.Msg!;
        }
    }
}
