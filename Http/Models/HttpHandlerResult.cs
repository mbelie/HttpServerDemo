using System.Net;

namespace Http.Models;

public record HttpHandlerResult(string Response, HttpStatusCode StatusCode, uint? TimeToLiveSeconds);