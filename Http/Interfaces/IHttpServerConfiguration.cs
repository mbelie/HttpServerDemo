namespace Http.Interfaces;

/// <summary>
///     Describes an HTTP server configuration
/// </summary>
public interface IHttpServerConfiguration
{
    /// <summary>
    ///     The port the server will be hosted on
    /// </summary>
    int Port { get; }

    /// <summary>
    ///     Specifies the user agent to use when making requests to a weather endpoint
    /// </summary>
    string UserAgent { get; }

    // TODO: Consider supporting max connection requests
}