using Microsoft.Extensions.Caching.Memory;
using YardOperations.Application.Caching;

namespace YardOperations.Infrastructure.Caching;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<T> GetOrCreateAsync<T>(string key,  Func<Task<T>> factory, TimeSpan ttl)
    {
        if (_memoryCache.TryGetValue<T>(key, out var cachedValue)) {return cachedValue!;}
        var value = await factory();
        _memoryCache.Set(key, value, ttl);
        return value;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}
