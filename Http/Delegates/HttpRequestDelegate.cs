using System.Net;

namespace Http.Models;

public delegate Task<HttpHandlerResult> HttpRequestDelegate(HttpListenerContext context);