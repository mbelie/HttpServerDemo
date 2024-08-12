using Http.Models;

namespace Http.Interfaces;

/// <summary>
///     Describes an HTTP server type
/// </summary>
public interface IHttpServer : IDisposable
{
    /// <summary>
    ///     Starts the server
    /// </summary>
    void Start();

    /// <summary>
    ///     Stops the server
    /// </summary>
    void Stop();

    /// <summary>
    ///     Registers an endpoint with the server
    /// </summary>
    /// <param name="endpointPattern">The Regex pattern of the endpoint to serve</param>
    /// <param name="handler">The handler that serves the endpoint</param>
    void RegisterEndpoint(string endpointPattern, HttpRequestDelegate handler);

    /// <summary>
    ///     Tells the server to use the supplied middleware
    /// </summary>
    /// <param name="middleware">The middleware to use</param>
    void Use(IHttpMiddleware middleware);
}