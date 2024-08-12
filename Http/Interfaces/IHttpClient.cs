namespace Http.Interfaces;

/// <summary>
///     An abstraction of an HTTP client
/// </summary>
public interface IHttpClient : IDisposable
{
    /// <summary>
    ///     Performs a get request for a given URL
    /// </summary>
    /// <param name="url">The URL to get</param>
    /// <returns>An awaitable IHttpResponse</returns>
    Task<IHttpResponse> Get(string url);
}