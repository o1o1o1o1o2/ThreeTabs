using System;
using System.Threading;
using Client.Libs.Networking;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Client.Weather.UI
{
    [ScreenView(typeof(WeatherTabScreenView))]
    internal class WeatherTabScreenPresenter : UIScreenPresenter
    {
        private readonly WeatherTabScreenView _screenView;
        private readonly IWeatherRepository _weatherRepository;

        private CancellationTokenSource _cts;
        private IQueuedRequestHandle<WeatherModel> _weatherRequest;
        private IQueuedRequestHandle<Texture2D> _weatherIconRequest;
        private bool _isFirstLoad = true;

        public WeatherTabScreenPresenter(
            WeatherTabScreenView screenView,
            IScreenManager screenManager,
            IWeatherRepository weatherRepository)
            : base(screenView, screenManager)
        {
            _screenView = screenView;
            _weatherRepository = weatherRepository;
        }

        public override ScreenStyle Style => ScreenStyle.Default;

        protected override UniTask OnOpenAsync()
        {
            _cts = new CancellationTokenSource();
            LoadForecastLoop(_cts.Token).Forget();
            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _weatherRequest?.Cancel();
            _weatherIconRequest?.Cancel();

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid LoadForecastLoop(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await LoadForecast(ct);
                    await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: ct);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask LoadForecast(CancellationToken ct)
        {
            if (_isFirstLoad)
            {
                _screenView.SetWeatherStatus("Loading weather...");
            }

            var request = _weatherRepository.GetForecast();
            _weatherRequest = request;

            try
            {
                var forecast = await request.Task.AttachExternalCancellation(ct);
                _screenView.SetWeatherStatus($"{GetDisplayPeriod(forecast.Name)} - {forecast.Temperature}{forecast.TemperatureUnit}");
                await LoadWeatherIcon(forecast.IconUrl, ct);
                _isFirstLoad = false;
            }
            catch (OperationCanceledException)
            {
                if (_isFirstLoad)
                {
                    _screenView.SetWeatherStatus("Loading cancelled.");
                }
            }
            catch (Exception exception)
            {
                _screenView.SetWeatherStatus(exception.Message);
            }
            finally
            {
                if (ReferenceEquals(_weatherRequest, request))
                    _weatherRequest = null;
            }
        }

        private async UniTask LoadWeatherIcon(string iconUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(iconUrl))
                return;

            _weatherIconRequest?.Cancel();
            var request = _weatherRepository.GetIcon(iconUrl);
            _weatherIconRequest = request;

            try
            {
                var texture = await request.Task.AttachExternalCancellation(ct);
                _screenView.SetWeatherIcon(texture);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (ReferenceEquals(_weatherIconRequest, request))
                    _weatherIconRequest = null;
            }
        }

        private static string GetDisplayPeriod(string period) =>
            string.Equals(period, "Today", StringComparison.OrdinalIgnoreCase) ? "Today" : period;
    }
}