using System.Net;

namespace Http.Interfaces;

/// <summary>
///     Describes an abstraction for a type that performs processing on an HTTP request and indicates through its return
///     type whether to proceed
/// </summary>
public interface IHttpMiddleware
{
    /// <summary>
    ///     Performs some custom process on an HTTP request
    /// </summary>
    /// <param name="context">The current HTTP context</param>
    /// <returns>An awaitable boolean indicating whether to proceed</returns>
    Task<bool> Handle(HttpListenerContext context);
}