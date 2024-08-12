using Newtonsoft.Json;

namespace WeatherService.Models.NationalWeatherService;

public class GridPointResponse
{
    [JsonProperty("properties")] public GridPointResponseProperties Properties { get; set; }
}