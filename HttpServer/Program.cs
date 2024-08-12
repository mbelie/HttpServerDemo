using System.Net;
using System.Text.RegularExpressions;
using Http;
using Http.Cache;
using Http.Interfaces;
using Http.Middleware;
using Http.Models;
using HttpServer.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherService.Interfaces.Services;
using WeatherService.Models;
using WeatherService.Services;

// Escape the forward slash as it's a special Regex character
// Example: http://localhost:9000/Weather/33.7070,-117.0845
const string WeatherEndpointPattern = "\\/Weather/.+";

// Example: /Weather/33.7070,-117.0845
const string ParametersPattern = ".+\\/(.+),(.+)";

const string UserAgentHeaderName = "User-Agent";

var parameterRegex = new Regex(ParametersPattern);

AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

var serviceProvider = Bootstrap();
var logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Program>();
var configuration = serviceProvider.GetService<IHttpServerConfiguration>();
var weatherService = serviceProvider.GetService<IWeatherService>();
var server = serviceProvider.GetService<IHttpServer>();

// NOTE: Middleware stubs for things like authentication and rate limiting
server.Use(new ApiKeyAuthentication());
server.Use(new RateLimiter());

server.RegisterEndpoint(WeatherEndpointPattern,
    async context =>
    {
        // TODO: Resolve the weather service and call it here instead
        var match = parameterRegex.Match(context.Request.Url!.OriginalString);

        if (match.Success)
        {
            try
            {
                var latitudeRaw = match.Groups[1].Value;
                var longitudeRaw = match.Groups[2].Value;

                logger.LogInformation($"Forecast requested for latitude: {latitudeRaw}, longitude: {longitudeRaw}");

                var latitude = Convert.ToSingle(latitudeRaw);
                var longitude = Convert.ToSingle(longitudeRaw);

                var result = await weatherService!.GetForecast(latitude, longitude);

                return result.Error != null
                    ? Error(result.Error!)
                    : new HttpHandlerResult(JsonConvert.SerializeObject(result.WeatherForecast!), HttpStatusCode.OK,
                        result.WeatherForecast!.TimeToLiveSeconds);
            }
            catch (Exception e)
            {
                return Error(e.Message);
            }
        }

        return Error("Longitude/Latitude parameters may be malformed");

        HttpHandlerResult Error(string error)
        {
            return new HttpHandlerResult(JsonConvert.SerializeObject(new WeatherError(error)),
                HttpStatusCode.InternalServerError, null);
        }
    });

server.Start();

Console.WriteLine($"Server listening on port {configuration.Port}");
Console.ReadLine();

Cleanup();

return;

IServiceProvider Bootstrap()
{
    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json");

    var configuration = builder.Build();

    var httpServerConfiguration = configuration.GetSection("HttpServer").Get<HttpServerConfiguration>();

    var httpClient = new HttpClient();
    var headers = new List<KeyValuePair<string, string>>
        { new(UserAgentHeaderName, httpServerConfiguration!.UserAgent) };

    return new ServiceCollection()
        .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug)
            .AddSimpleConsole(_ => { }))
        .AddMemoryCache()
        .AddSingleton<IHttpClient>(new HttpClientWrapper(httpClient, headers))
        .AddSingleton<IHttpServerConfiguration>(httpServerConfiguration)
        .AddSingleton<IHttpResponseCache, MockDistributedHttpResponseCache>()
        .AddSingleton<IHttpServer, SimpleHttpServer>()
        .AddSingleton<IWeatherService, NationalWeatherService>()
        .BuildServiceProvider();
}

void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    var error = e.ExceptionObject.ToString();

    Console.WriteLine(error);
    Console.ReadLine();
    Environment.Exit(1);
}

void Cleanup()
{
    server.Stop();
    server.Dispose();

    weatherService?.Dispose();
}