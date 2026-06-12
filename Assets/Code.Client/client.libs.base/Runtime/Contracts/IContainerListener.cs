using Zenject;

namespace Client.Libs.Contracts
{
    public interface IContainerListener
    {
        void OnInstall(DiContainer container);
        void OnUninstall(DiContainer container);
    }
}