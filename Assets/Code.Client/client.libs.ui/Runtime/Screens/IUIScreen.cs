using System;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;

namespace Client.Libs.UI.Screens
{
    public interface IScreenArgs
    {
    }

    public interface IUIScreenSetup
    {
    }

    public interface IUIScreenSetup<in TArgs> : IUIScreenSetup where TArgs : IScreenArgs
    {
        UniTask SetupScreen(TArgs args);
    }

    public interface IUIScreen
    {
        ScreenState State { get; }
        ScreenStyle Style { get; }

        void Close();
        internal UniTask OpenInternal(Func<UniTask> setVisible, bool forceRefresh = false);
        UniTask CloseAsync();
        internal UniTask CloseInternal(Action doAfterHide = null);
    }

    public interface IUiScreenWithResult
    {
        object GetResult();
    }
}