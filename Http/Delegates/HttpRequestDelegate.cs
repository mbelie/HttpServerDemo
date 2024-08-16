using System.Net;
using Http.Models;

namespace Http.Delegates;

public delegate Task<HttpHandlerResult> HttpRequestDelegate(HttpListenerContext context);