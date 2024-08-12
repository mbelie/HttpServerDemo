using WeatherService.Interfaces.Models;

namespace WeatherService.Models;

public record WeatherForecast(
    float Latitude,
    float Longitude,
    DateTime Timestamp,
    float Temperature,
    TemperatureUnit TemperatureUnit,
    string ShortForecast,
    WeatherCharacterization Characterization,
    uint? TimeToLiveSeconds
) : IWeatherForecast;