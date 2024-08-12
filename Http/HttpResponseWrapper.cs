using System.Net;
using Http.Interfaces;

namespace Http;

public class HttpResponseWrapper : IHttpResponse
{
    private readonly HttpResponseMessage _httpResponseMessage;

    public HttpResponseWrapper(HttpResponseMessage httpResponseMessage)
    {
        _httpResponseMessage = httpResponseMessage ?? throw new ArgumentNullException(nameof(httpResponseMessage));
    }

    public HttpStatusCode StatusCode => _httpResponseMessage.StatusCode;

    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers =>
        _httpResponseMessage.Headers.Select(x => new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value));

    public async Task<string> GetBody()
    {
        return await _httpResponseMessage.Content.ReadAsStringAsync();
    }
}