using Http.Interfaces;

namespace HttpServer.Configuration;

public class HttpServerConfiguration : IHttpServerConfiguration
{
    public int Port { get; set; }

    public string UserAgent { get; set; }
}