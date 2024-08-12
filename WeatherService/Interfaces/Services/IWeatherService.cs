using WeatherService.Interfaces.Models;

namespace WeatherService.Interfaces.Services;

/// <summary>
///     Describes an abstraction for a generic weather service
/// </summary>
public interface IWeatherService : IDisposable
{
    /// <summary>
    ///     Gets a weather forecast for the given latitude/longitude pair
    /// </summary>
    /// <param name="latitude">The latitude of a location</param>
    /// <param name="longitude">The longitude of a location</param>
    /// <returns>
    ///     An awaitable IWeatherForecast result
    /// </returns>
    Task<IWeatherForecastResult> GetForecast(float latitude, float longitude);
}