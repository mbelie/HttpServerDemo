using WeatherService.Interfaces.Models;

namespace WeatherService.Models;

public record WeatherForecastResult(
    IWeatherForecast? WeatherForecast,
    string? Error = null) : IWeatherForecastResult;