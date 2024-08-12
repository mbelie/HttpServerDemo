using System.Net;
using Http.Interfaces;

namespace Http.Middleware;

public class ApiKeyAuthentication : IHttpMiddleware
{
    public async Task<bool> Handle(HttpListenerContext context)
    {
        // TODO: Future work. Perform authorization and only call Handle when authorization succeeds. Otherwise, closeout the response
        return true;
    }
}