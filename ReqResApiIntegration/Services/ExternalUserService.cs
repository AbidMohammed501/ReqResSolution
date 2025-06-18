using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ReqResApiIntegration.Config;
using ReqResApiIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReqResApiIntegration.Services
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ReqResOptions _options;

        public ExternalUserService(HttpClient httpClient, IOptions<ReqResOptions> options, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _cache = cache;
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _options.ApiKey);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/{userId}");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SingleUserResponse>(content);
                return result?.Data;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Error fetching user by ID", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            const string cacheKey = "all_users";

            if (_cache.TryGetValue(cacheKey, out List<User> cachedUsers))
                return cachedUsers;

            var allUsers = new List<User>();
            int page = 1;
            int totalPages = 1;

            try
            {
                do
                {
                    var response = await _httpClient.GetAsync($"users?page={page}");
                    if (!response.IsSuccessStatusCode) break;

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<UserDataResponse>(content);
                    if (result != null)
                    {
                        totalPages = result.TotalPages;
                        allUsers.AddRange(result.Data);
                    }
                    page++;
                }
                while (page <= totalPages);

                _cache.Set(cacheKey, allUsers, TimeSpan.FromSeconds(_options.CacheExpirationSeconds));
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Error fetching all users", ex);
            }

            return allUsers;
        }
    }
}
