using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskFlow.Application.Common;
namespace TaskFlow.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cachedValue = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedValue))
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };

            var jsonData = JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(key, jsonData, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }

}
