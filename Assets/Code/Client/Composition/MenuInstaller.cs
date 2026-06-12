using Client.Clicker;
using Client.Clicker.Configs;
using Client.Dogs;
using Client.Dogs.UI;
using Client.Infrastructure;
using Client.Libs.Installers;
using Client.Libs.Networking;
using Client.Weather;
using UnityEngine;

namespace Client.Composition
{
    internal sealed class MenuInstaller : SceneInstaller<MenuInstaller>
    {
        [SerializeField] private ClickerConfig _clickerConfig;

        protected override void OnInstallBindings()
        {
            Container.Bind<IClickerConfig>().FromInstance(_clickerConfig);
            Container.BindInterfacesTo<QueuedRequestRunner>().AsSingle();
            Container.BindInterfacesAndSelfTo<ClickerController>().AsSingle();
            Container.BindInterfacesAndSelfTo<QueuedHttpClient>().AsSingle();
            Container.BindInterfacesTo<DogBreedsRepository>().AsSingle();
            Container.BindInterfacesAndSelfTo<DogBreedDetailsPopupState>().AsSingle();
            Container.Bind<IWeatherRepository>().To<WeatherRepository>().AsSingle();
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose() =>
            Container.TryResolve<IQueuedRequestRunner>()?.Dispose();
    }
}