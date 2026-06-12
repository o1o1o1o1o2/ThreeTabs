using Client.Libs.UI.Contracts;
using Client.Libs.UI.Screens;
using Cysharp.Threading.Tasks;
using Shared.Libs.Utils;

namespace Client.Libs.UI.Extensions
{
    public struct ScreenScope<TScreen> where TScreen : UIScreenPresenter
    {
        internal readonly IScreenManager ScreenManager;

        internal ScreenScope(IScreenManager screenManager) =>
            ScreenManager = screenManager;
    }

    public static class ScreenManagerExtensions
    {
        public static ScreenScope<TScreen> Screen<TScreen>(this IScreenManager screenManager) where TScreen : UIScreenPresenter =>
            new(screenManager);

        public static UniTask Open<TScreen>(this ScreenScope<TScreen> screen) where TScreen : UIScreenPresenter =>
            screen.ScreenManager.Show(TypeOf<TScreen>.Raw);

        public static UniTask Close<TScreen>(this ScreenScope<TScreen> screen) where TScreen : UIScreenPresenter =>
            screen.ScreenManager.CloseLast(TypeOf<TScreen>.Raw);
    }
}