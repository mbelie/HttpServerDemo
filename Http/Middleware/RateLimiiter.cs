using System.Net;
using Http.Interfaces;

namespace Http.Middleware;

public class RateLimiter : IHttpMiddleware
{
    public async Task<bool> Handle(HttpListenerContext context)
    {
        // TODO: Future work. Perform rate limiting and only call Handle when rate limiting succeeds. Otherwise, closeout the response
        return true;
    }
}