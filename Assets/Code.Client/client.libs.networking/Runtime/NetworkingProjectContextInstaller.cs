using Client.Libs.Installers;

namespace Client.Libs.Networking
{
    public class NetworkingProjectContextInstaller : ProjectContextInstaller<NetworkingProjectContextInstaller>
    {
        protected override void OnInstallBindings() =>
            Container.BindInterfacesTo<QueuedRequestRunner>().AsSingle();

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }
    }
}