using Http.Interfaces;

namespace Http;

public class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _httpClient;

    private bool _isDisposed;

    public HttpClientWrapper(HttpClient httpClient, IEnumerable<KeyValuePair<string, string>>? headers)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        if (headers == null)
        {
            return;
        }

        foreach (var header in headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public async Task<IHttpResponse> Get(string url)
    {
        var result = await _httpClient.GetAsync(url);

        return new HttpResponseWrapper(result);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    private void Dispose(bool isDisposing)
    {
        if (!isDisposing)
        {
            return;
        }

        _httpClient.Dispose();
    }
}