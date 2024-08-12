namespace WeatherService.Interfaces.Models;

/// <summary>
///     Describes an abstraction for a generic weather forecast result
/// </summary>
public interface IWeatherForecastResult
{
    /// <summary>
    ///     Optional forecast object. Non-null upon success.
    /// </summary>
    IWeatherForecast? WeatherForecast { get; }

    /// <summary>
    ///     Optional error. Non-null when an error has occurred
    /// </summary>
    string? Error { get; }
}