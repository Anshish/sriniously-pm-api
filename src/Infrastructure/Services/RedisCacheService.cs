using System.Text.Json;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services
{
    public class RedisCacheService(IDistributedCache cache) : ICacheService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive=true
        };

        public async Task<T?> GetAsync<T>(string key,CancellationToken ct = default)
        {
            var cachedString=await cache.GetStringAsync(key,ct);
            if (string.IsNullOrWhiteSpace(cachedString))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(cachedString,JsonOptions);
        }

        public async Task SetAsync<T>(string key,T value,TimeSpan? expiration,CancellationToken ct = default)
        {
            var json=JsonSerializer.Serialize(value,JsonOptions);
            var options=new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow=expiration??TimeSpan.FromMinutes(5)
            };
            await cache.SetStringAsync(key,json,options,ct);
        }
        public async Task RemoveAsync(string key,CancellationToken ct = default)
        {
            await cache.RemoveAsync(key,ct);
        }
    }
}