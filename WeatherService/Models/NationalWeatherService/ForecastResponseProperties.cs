using Newtonsoft.Json;

namespace WeatherService.Models.NationalWeatherService;

public class ForecastResponseProperties
{
    [JsonProperty("periods")] public Period[] Periods { get; set; }
}