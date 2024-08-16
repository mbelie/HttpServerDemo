using Http.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Http.Cache;

/// <summary>
///     NOTE: This type is for demo purposes only. Prefer a production distributed cache like Memcached or Redis
///     in order to support dynamic scaling/scaling out
/// </summary>
public class MockDistributedHttpResponseCache : IHttpResponseCache
{
    private readonly IMemoryCache _cache;

    public MockDistributedHttpResponseCache(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public Task Put(KeyValuePair<string, object> keyValuePair, uint? timeToLiveSeconds)
    {
        using var entry = _cache.CreateEntry(keyValuePair.Key);
        entry.Value = keyValuePair.Value;

        if (timeToLiveSeconds != null)
        {
            entry.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(timeToLiveSeconds.Value);
        }

        return Task.CompletedTask;
    }

    public Task<object?> Get(string key)
    {
        return Task.FromResult(_cache.Get(key));
    }
}