using Client.App.Internal;
using Client.Libs.Installers;
using Client.Menu;

namespace Client.Composition
{
    public sealed class AppInstaller : SceneInstaller<AppInstaller>
    {
        protected override void OnInstallBindings()
        {
            Container.BindInstance(transform);
            Container.BindInterfacesAndSelfTo<MenuState>().AsSingle();
            Container.BindInterfacesTo<UnityApplication>().AsSingle();
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }
    }
}