namespace Http.Interfaces;

/// <summary>
///     Abstraction for a type that caches HTTP responses
/// </summary>
public interface IHttpResponseCache
{
    /// <summary>
    ///     Puts/updates an object to the cache
    /// </summary>
    /// <param name="keyValuePair">The key value pair to put</param>
    /// <param name="timeToLiveSeconds">
    ///     An optional TTL indicated that indicates how long the cached item is valid for.
    ///     Null indicates no expiration
    /// </param>
    /// <returns>An awaitable</returns>
    Task Put(KeyValuePair<string, object> keyValuePair, uint? timeToLiveSeconds);

    /// <summary>
    ///     Gets a value out of the cache
    /// </summary>
    /// <param name="key">The key of the object to return</param>
    /// <returns>
    ///     An awaitable with a nullable object result
    /// </returns>
    Task<object?> Get(string key);
}