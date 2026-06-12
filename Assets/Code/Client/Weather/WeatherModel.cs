using Newtonsoft.Json;

namespace Client.Weather
{
    internal readonly struct WeatherModel
    {
        [JsonConstructor]
        public WeatherModel(
            [JsonProperty("icon")] string iconUrl,
            [JsonProperty("name")] string name,
            [JsonProperty("temperature")] int temperature,
            [JsonProperty("temperatureUnit")] string temperatureUnit)
        {
            IconUrl = iconUrl;
            Name = name;
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
        }

        public string IconUrl { get; }
        public string Name { get; }
        public int Temperature { get; }
        public string TemperatureUnit { get; }
    }
}