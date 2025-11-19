using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using TaskFlow.Application.Common;

namespace TaskFlow.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var cachedValue = await _cache.GetStringAsync(key);

                if (string.IsNullOrEmpty(cachedValue))
                    return default!;

                return JsonSerializer.Deserialize<T>(cachedValue)!;
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable when reading key {Key}. Returning default.", key);
                return default!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected cache read failure for key {Key}", key);
                return default!;
            }
        }

        public async Task SetAsync<T>(string key, T data, TimeSpan? expiry = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
                };

                var jsonData = JsonSerializer.Serialize(data);
                await _cache.SetStringAsync(key, jsonData, options);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable when writing key {Key}. Skipping cache.", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected cache write failure for key {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogWarning(ex, "Redis unavailable when removing key {Key}.", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected cache remove failure for key {Key}", key);
            }
        }
    }
}
