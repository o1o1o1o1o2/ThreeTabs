using Client.App.Configs;
using Client.App.Internal;
using Client.Libs.Installers;
using UnityEngine;

namespace Client.Composition
{
    internal sealed class AppProjectContextInstaller : ProjectContextInstaller<AppProjectContextInstaller>
    {
        [SerializeField] private SceneConfig _sceneConfig;

        protected override void OnInstallBindings()
        {
            Container.Bind<ISceneConfig>().FromInstance(_sceneConfig);
            Container.BindInterfacesTo<ApplicationLoader>().AsSingle();
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }
    }
}