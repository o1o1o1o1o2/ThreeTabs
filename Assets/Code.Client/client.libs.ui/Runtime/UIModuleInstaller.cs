using Client.Libs.Installers;
using Client.Libs.UI.Cameras;
using Client.Libs.UI.Config;
using Client.Libs.UI.Internal;
using Client.Libs.UI.Views;
using UnityEngine;

namespace Client.Libs.UI
{
    public class UIModuleInstaller : SceneInstaller<UIModuleInstaller>
    {
        [Header("Views"), SerializeField]
        private UIScreenLockView _screenLockView;

        [Header("Configs"), SerializeField]
        private UIGlobals _uiGlobals;

        protected override void OnInstallBindings()
        {
            Container.Bind<IUIGlobals>().FromInstance(_uiGlobals);

            Container.BindInterfacesTo<ScreenLocker>().AsSingle().NonLazy();
            Container.Bind<UIScreenLockView>().FromInstance(_screenLockView).WhenInjectedInto<ScreenLocker>();

            Container.Bind<ScreenLayerManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<UIScreenHistoryStack>().AsSingle();
            Container.BindInterfacesTo<ScreenManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIScreenLoader>().AsSingle();
        }

        protected override void OnInitialize()
        {
            var globals = Container.Resolve<IUIGlobals>();
            globals.ApplyUIScale();
        }

        protected override void OnDispose()
        {
        }
    }
}