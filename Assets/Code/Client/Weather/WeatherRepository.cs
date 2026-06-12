using System;
using Client.Infrastructure;
using Client.Libs.Networking;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Client.Weather
{
    internal sealed class WeatherRepository : IWeatherRepository
    {
        private const string FORECAST_URL = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        private readonly QueuedHttpClient _httpClient;

        public WeatherRepository(QueuedHttpClient httpClient) =>
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public IQueuedRequestHandle<WeatherModel> GetForecast()
        {
            var httpRequest = _httpClient.GetText(FORECAST_URL, "weather forecast");
            return new MappedQueuedRequestHandle<string, WeatherModel>(httpRequest, ParseForecast);
        }

        public IQueuedRequestHandle<Texture2D> GetIcon(string iconUrl) =>
            _httpClient.Enqueue(ct => LoadIcon(iconUrl, ct), "weather icon");

        private static WeatherModel ParseForecast(string json)
        {
            var root = JObject.Parse(json);
            var periodToken = root["properties"]?["periods"]?.First;

            return periodToken == null
                ? throw new InvalidOperationException("Weather response does not contain forecast periods.")
                : JsonConvert.DeserializeObject<WeatherModel>(periodToken.ToString());
        }

        private static async UniTask<Texture2D> LoadIcon(string iconUrl, System.Threading.CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(iconUrl))
                throw new ArgumentException("Weather icon url is empty.", nameof(iconUrl));

            using var request = UnityWebRequestTexture.GetTexture(iconUrl);
            await request.SendWebRequest().ToUniTask(cancellationToken: ct);

            return request.result != UnityWebRequest.Result.Success
                ? throw new InvalidOperationException($"{request.responseCode}: {request.error}")
                : DownloadHandlerTexture.GetContent(request);
        }
    }
}