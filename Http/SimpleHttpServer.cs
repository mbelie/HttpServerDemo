using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Http.Delegates;
using Http.Interfaces;
using Microsoft.Extensions.Logging;

namespace Http;

public class SimpleHttpServer : IHttpServer
{
    private const string GetMethodName = "get";

    private readonly IHttpResponseCache _cache;

    private readonly Dictionary<string, HttpRequestDelegate> _delegateMap = new();

    // TODO: Abstract with interface
    private readonly HttpListener _listener;

    private readonly ILogger<SimpleHttpServer> _logger;

    private readonly HashSet<IHttpMiddleware> _middleware = new();

    private CancellationTokenSource? _cancellationTokenSource;

    private bool _isDisposed;

    public SimpleHttpServer(IHttpServerConfiguration configuration,
        IHttpResponseCache cache,
        ILogger<SimpleHttpServer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));

        // TODO: Abstract with interface
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://*:{configuration.Port}/");
    }

    public void Start()
    {
        if (_listener.IsListening)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        _listener.Start();

        Task.Run(async () =>
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                HttpListenerContext context;
                
                try
                {
                    // TODO: Consider queuing tasks and starting max N workers to process
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (HttpListenerException e)
                {
                    _logger.LogInformation("Server stopped");
                    break;
                }

                // NOTE: Only supporting gets in this HTTP server. Requires extension to support other verbs.
                if (context.Request.HttpMethod.ToLower() != GetMethodName)
                {
                    _logger.LogInformation($"{context.Request.HttpMethod} not supported");
                    continue;
                }

                var endpointHandler = GetHandler(context.Request.Url);
                if (endpointHandler == null)
                {
                    _logger.LogInformation($"No handler for path \"{context.Request.Url}\"");
                    Process404(context);
                    continue;
                }

                // Don't await so that we can begin waiting for the next incoming request
                ProcessRequest(context, endpointHandler);
            }
        }, _cancellationTokenSource.Token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _listener.Stop();
    }

    public void Use(IHttpMiddleware middleware)
    {
        if (_listener.IsListening)
        {
            return;
        }

        _middleware.Add(middleware);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void RegisterEndpoint(string endpointPattern, HttpRequestDelegate handler)
    {
        if (_listener.IsListening)
        {
            return;
        }

        _delegateMap[endpointPattern] = handler;
    }

    private Task Process404(HttpListenerContext context)
    {
        return Task.Run(async () => { await WriteResponse(context, "Path not supported", HttpStatusCode.NotFound); },
            _cancellationTokenSource!.Token);
    }

    private void Dispose(bool isDisposing)
    {
        if (!isDisposing)
        {
            return;
        }

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        ((IDisposable)_listener).Dispose();
    }

    private HttpRequestDelegate? GetHandler(Uri? uri)
    {
        if (uri == null || _delegateMap.Count == 0)
        {
            return null;
        }

        var key = _delegateMap.Keys.FirstOrDefault(pathPattern =>
        {
            var regex = new Regex(pathPattern);
            return regex.IsMatch(uri.OriginalString);
        });

        return key == null
            ? null
            : _delegateMap[key];
    }

    private Task ProcessRequest(HttpListenerContext context, HttpRequestDelegate handler)
    {
        return Task.Run(async () =>
        {
            foreach (var middleware in _middleware)
            {
                var proceed = await middleware.Handle(context);
                if (!proceed)
                {
                    _logger.LogInformation(
                        $"Middleware {middleware.GetType()} indicated the request pipeline should be terminated");
                    return;
                }
            }

            var cacheKey = context.Request.Url!.OriginalString;

            var cachedResponse = await _cache.Get(cacheKey);

            if (cachedResponse != null)
            {
                _logger.LogDebug($"Found a cached result for \"{cacheKey}\"");
                await WriteResponse(context, cachedResponse.ToString() ?? string.Empty, HttpStatusCode.OK);
            }
            else
            {
                var result = await handler(context);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    // No need to await this
                    _cache.Put(new KeyValuePair<string, object>(cacheKey, result.Response), result.TimeToLiveSeconds);
                }

                await WriteResponse(context, result.Response, result.StatusCode);
            }
        });
    }

    private async Task WriteResponse(HttpListenerContext context, string response, HttpStatusCode statusCode)
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentLength64 = responseBytes.Length;
        context.Response.ContentType = "application/json";
        await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        context.Response.OutputStream.Close();
        context.Response.Close();
    }
}