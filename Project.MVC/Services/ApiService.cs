using Project.MVC.Models;
using System.Text;
using System.Text.Json;

namespace Project.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            _httpClient.BaseAddress =
                new Uri(configuration["ApiSettings:BaseUrl"]!);
        }

        // HTTP METHODS

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>( string url, TRequest data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task PostAsync<TRequest>( string url, TRequest data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }
        public async Task PutAsync<T>(string url, T data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        //  SESSION HELPERS 

        public void SetCurrentUser(UserSession user)
        {
            var json = JsonSerializer.Serialize(user);
            _httpContextAccessor.HttpContext!
                .Session.SetString("CurrentUser", json);
        }

        public UserSession? GetCurrentUser()
        {
            var json = _httpContextAccessor.HttpContext!
                .Session.GetString("CurrentUser");

            return string.IsNullOrEmpty(json)
                ? null
                : JsonSerializer.Deserialize<UserSession>(json);
        }

        public void ClearSession()
        {
            _httpContextAccessor.HttpContext!.Session.Clear();
        }
    }
}