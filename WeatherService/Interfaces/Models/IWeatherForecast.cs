using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeatherService.Interfaces.Models;

/// <summary>
///     Describes a weather forecast for geographic location
/// </summary>
public interface IWeatherForecast
{
    /// <summary>
    ///     The latitude requested
    /// </summary>
    float Latitude { get; }

    /// <summary>
    ///     The longitude requested
    /// </summary>
    float Longitude { get; }

    /// <summary>
    ///     The numeric temperature of the location at the time of the request
    /// </summary>
    float Temperature { get; }

    /// <summary>
    ///     The time of the response
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    ///     The unit of the temperature (F/C)
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    TemperatureUnit TemperatureUnit { get; }

    /// <summary>
    ///     A human-readable string that describes the weather
    /// </summary>
    string ShortForecast { get; }

    /// <summary>
    ///     A characterization of the weather based on the temperature
    /// </summary>
    /// <remarks>
    ///     Under 55: Cold,
    ///     Under 85: Moderate,
    ///     At/Over 85: Hot
    /// </remarks>
    [JsonConverter(typeof(StringEnumConverter))]
    WeatherCharacterization Characterization { get; }

    /// <summary>
    ///     Optional seconds value that when provided, indicates how long the response is valid for
    /// </summary>
    uint? TimeToLiveSeconds { get; }
}