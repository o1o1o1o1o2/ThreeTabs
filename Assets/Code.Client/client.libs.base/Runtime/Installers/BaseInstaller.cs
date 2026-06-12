using System;
using Client.Libs.Contracts;
using Client.Libs.Extensions;
using Shared.Libs.Utils;
using Zenject;

namespace Client.Libs.Installers
{
    public abstract partial class BaseInstaller<TDerived> : MonoInstaller<TDerived>, IInitializable, IDisposable
        where TDerived : MonoInstaller<TDerived>
    {
        protected readonly LogCat Logger = new(TypeOf<TDerived>.Raw.Name, LogFormatter.Color.darkgreen);

        public void Initialize()
        {
            Logger.LogDev("Initialize");

            Container.QueueAsyncInitializers();

            foreach (var containerListener in Container.ResolveAll<IContainerListener>())
                containerListener.OnInstall(Container);

            OnInitialize();
        }

        public void Dispose()
        {
            Logger.LogDev("Dispose");

            foreach (var containerListener in Container.ResolveAll<IContainerListener>())
                containerListener.OnUninstall(Container);

            OnDispose();
        }

        public override void InstallBindings()
        {
            Logger.LogDev("InstallBindings");

            if (GetType() != TypeOf<TDerived>.Raw)
                throw new Exception($"{GetType().FullName}: invalid generic base param: must be <{GetType().Name}> but actual is <{TypeOf<TDerived>.Raw.Name}>!");

            Container.BindInterfacesTo(GetType()).FromInstance(this);

            OnInstallBindings();
        }

        protected abstract void OnInstallBindings();

        protected abstract void OnInitialize();
        protected abstract void OnDispose();
    }
}