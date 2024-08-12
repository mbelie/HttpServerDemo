using System.Text.RegularExpressions;
using Http.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherService.Interfaces.Models;
using WeatherService.Interfaces.Services;
using WeatherService.Models;
using WeatherService.Models.NationalWeatherService;

namespace WeatherService.Services;

public class NationalWeatherService : IWeatherService
{
    private const string GridPointEndpointFormatter = "https://api.weather.gov/points/{0},{1}";
    private const string CacheControlHeaderName = "Cache-Control";

    // Example: public, max-age=3600, s-maxage=3600
    private const string SharedCacheMaxAgePattern = @".*s-maxage=(\d+)";

    private const float ColdThreshold = 55f;
    private const float ModerateThreshold = 85f;

    private readonly Regex _cacheControlRegex = new(SharedCacheMaxAgePattern);
    private readonly IHttpClient _httpClient;
    private readonly ILogger<NationalWeatherService> _logger;

    private bool _isDisposed;

    public NationalWeatherService(IHttpClient httpClient, ILogger<NationalWeatherService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IWeatherForecastResult> GetForecast(float latitude, float longitude)
    {
        var gridPointUrl = string.Format(GridPointEndpointFormatter, latitude, longitude);
        var gridPointResponseMessage = await _httpClient.Get(gridPointUrl);

        if (!gridPointResponseMessage.WasSuccessful)
        {
            return Error(
                $"StatusCode: {gridPointResponseMessage.StatusCode}. Failed to get the grid point for {latitude}, {longitude}");
        }

        var body = await gridPointResponseMessage.GetBody();
        var gridPointResponse = ParseResponse<GridPointResponse>(body);
        if (gridPointResponse.Response == null)
        {
            return Error(gridPointResponse.Error!);
        }

        // NOTE: Happens with malformed User-Agent
        if (string.IsNullOrEmpty(gridPointResponse.Response!.Properties.ForecastUrl))
        {
            return Error($"Expected forecast URL is empty for {latitude}, {longitude}");
        }

        var forecastResponseMessage = await _httpClient.Get(gridPointResponse.Response!.Properties.ForecastUrl);
        if (!forecastResponseMessage.WasSuccessful)
        {
            return Error(
                $"StatusCode: {gridPointResponseMessage.StatusCode}. Failed to get forecast data for {latitude}, {longitude}");
        }

        body = await forecastResponseMessage.GetBody();
        var forecastResponse = ParseResponse<ForecastResponse>(body);
        if (forecastResponse.Response == null)
        {
            return Error(forecastResponse.Error!);
        }

        // TODO: Could extend the API to accept another parameter to indicate which period to return
        var period = forecastResponse.Response!.Properties.Periods.SingleOrDefault(x => x.Number == 1);

        if (period == null)
        {
            return Error("Period 1 is missing from the forecast response");
        }

        var cacheControlValue = forecastResponseMessage.Headers.FirstOrDefault(x => x.Key == CacheControlHeaderName)
            .Value?.FirstOrDefault();

        var timeToLiveSeconds = ComputeTimeToLive(cacheControlValue);

        var weatherForecast = new WeatherForecast(latitude, longitude, DateTime.UtcNow,
            period.Temperature, period.TemperatureUnit, period.ShortForecast, CharacterizeTemperature(period),
            timeToLiveSeconds);

        return new WeatherForecastResult(weatherForecast);
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

    private (T? Response, string? Error) ParseResponse<T>(string body) where T : class
    {
        try
        {
            var response = JsonConvert.DeserializeObject<T>(body);

            return (response, null);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to deserialize {typeof(T)}");
            return (null, e.Message);
        }
    }

    private WeatherCharacterization CharacterizeTemperature(Period period)
    {
        var value = period.TemperatureUnit == TemperatureUnit.F
            ? period.Temperature
            : (period.Temperature - 32f) * (5 / 9f);

        if (value < ColdThreshold)
        {
            return WeatherCharacterization.Cold;
        }

        return value < ModerateThreshold
            ? WeatherCharacterization.Moderate
            : WeatherCharacterization.Hot;
    }

    private uint? ComputeTimeToLive(string? cacheControlValue)
    {
        if (cacheControlValue == null)
        {
            return null;
        }

        var match = _cacheControlRegex.Match(cacheControlValue);

        var rawSeconds = match.Success
            ? match.Groups[1].Value
            : null;

        var wasParsed = uint.TryParse(rawSeconds, out var seconds);

        return wasParsed
            ? seconds
            : null;
    }

    private WeatherForecastResult Error(string message)
    {
        return new WeatherForecastResult(null, message);
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