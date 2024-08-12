using Http.Interfaces;

namespace HttpServerConsole.Configuration;

public class HttpServerConfiguration : IHttpServerConfiguration
{
    public int Port { get; set; }

    public string UserAgent { get; set; }
}