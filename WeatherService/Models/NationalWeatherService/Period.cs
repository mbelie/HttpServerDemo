using Newtonsoft.Json;

namespace WeatherService.Models.NationalWeatherService;

public class Period
{
    [JsonProperty("number")] public int Number { get; set; }

    [JsonProperty("shortForecast")] public string ShortForecast { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    // TODO: Determine values and convert to enum
    [JsonProperty("temperatureUnit")] public string TemperatureUnitRaw { get; set; }

    // ISO 8601 UTC 
    [JsonProperty("startTime")] public string StartTimeRaw { get; set; }

    // ISO 8601 UTC 
    [JsonProperty("endTime")] public string EndTimeRaw { get; set; }

    [JsonProperty("temperature")] public float Temperature { get; set; }

    public DateTime EndTime => DateTime.Parse(EndTimeRaw);

    public DateTime StartTime => DateTime.Parse(StartTimeRaw);

    public TemperatureUnit TemperatureUnit => TemperatureUnitRaw.ToLower().Equals("f")
        ? TemperatureUnit.F
        : TemperatureUnit.C;
}