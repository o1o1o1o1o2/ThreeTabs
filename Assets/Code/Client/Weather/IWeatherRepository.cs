using Client.Libs.Networking;
using UnityEngine;

namespace Client.Weather
{
    internal interface IWeatherRepository
    {
        IQueuedRequestHandle<WeatherModel> GetForecast();
        IQueuedRequestHandle<Texture2D> GetIcon(string iconUrl);
    }
}