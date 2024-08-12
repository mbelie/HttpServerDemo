using Newtonsoft.Json;

namespace WeatherService.Models.NationalWeatherService;

public class ForecastResponse
{
    [JsonProperty("properties")] public ForecastResponseProperties Properties { get; set; }
}