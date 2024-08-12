using System.Net;

namespace Http.Interfaces;

/// <summary>
///     Describes an abstraction of an HTTP response
/// </summary>
public interface IHttpResponse
{
    /// <summary>
    ///     The HTTP status code of the response
    /// </summary>
    HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     Exposes the headers of the response
    /// </summary>
    IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }

    /// <summary>
    ///     Indicates if the request was successful or not
    /// </summary>
    bool WasSuccessful => StatusCode == HttpStatusCode.OK;

    /// <summary>
    ///     Gets the response body
    /// </summary>
    /// <returns>An awaitable string</returns>
    Task<string> GetBody();
}