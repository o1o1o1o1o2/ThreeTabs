using System;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Views;
using Cysharp.Threading.Tasks;

namespace Client.Libs.UI.Internal
{
    public abstract class UIScreenView : UIView
    {
        internal IScreenLocker ScreenLocker => ScreenLockerInternal;

        internal void PreWarmScreen() =>
            PreWarm();

        internal UniTask ShowScreenAsync(Func<UniTask> setVisible) =>
            ShowViewAsync(setVisible);

        internal UniTask HideScreenAsync() =>
            HideViewAsync();
    }
}