using System;
using Client.Libs.UI.Cameras;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Client.Libs.UI.Views;
using Cysharp.Threading.Tasks;

namespace Client.Libs.UI.Internal
{
    internal sealed class ScreenManager : IScreenManager
    {
        private readonly IScreenLocker _screenLocker;
        private readonly UIScreenLoader _screenLoader;
        private readonly ScreenLayerManager _screenLayerManager;
        private readonly UIScreenHistoryStack _historyStack;

        public ScreenManager(
            IScreenLocker screenLocker,
            UIScreenLoader screenLoader,
            UIScreenHistoryStack historyStack,
            ScreenLayerManager screenLayerManager)
        {
            _screenLocker = screenLocker;
            _screenLoader = screenLoader;
            _historyStack = historyStack ?? throw new ArgumentNullException(nameof(historyStack));
            _screenLayerManager = screenLayerManager;
        }

        public void Register(UIView view) =>
            view.BeforeShown += ViewOnBeforeShown;

        public void Unregister(UIView view) =>
            view.BeforeShown -= ViewOnBeforeShown;

        async UniTask<UIScreenEntry> IScreenManager.Show(Type screenType)
        {
            var opened = _historyStack.LastOrDefault(screenType);
            if (opened != null)
                return opened;

            var tag = new ScreenLockTag($"Screen${screenType.Name}");
            try
            {
                _screenLocker.LockScreen(tag);

                var screenInfo = await _screenLoader.GetOrLoadScreenView(this, screenType);

                if (screenInfo?.ScreenPresenter == null)
                    return null;

                if (screenInfo.ScreenPresenter.Style.HasFlag(ScreenStyle.FullScreen))
                    await CloseAllScreensInternal();

                var screenStackEntry = new UIScreenEntry(screenInfo);
                _historyStack.Add(screenStackEntry);

                await screenInfo.ScreenPresenter.OpenInternal(() => SetObjectsVisible(screenInfo, true));
                await UpdateVisualStack();

                return screenStackEntry;
            }
            catch
            {
                var entry = _historyStack.LastOrDefault(screenType);
                if (entry != null)
                    _historyStack.Remove(entry);

                throw;
            }
            finally
            {
                _screenLocker.UnlockScreen(tag);
            }
        }

        public async UniTask CloseLast(Type screenType)
        {
            var entry = _historyStack.LastOrDefault(screenType);
            if (entry == null)
                return;

            await HideScreenEntry(entry);
        }

        private void ViewOnBeforeShown(UIView view) =>
            UpdateVisualStack().Forget();

        private async UniTask CloseAllScreensInternal()
        {
            while (_historyStack.Count > 0)
                await HideScreenEntry(_historyStack.Last());
        }

        private async UniTask HideScreenEntry(UIScreenEntry screenStackEntry)
        {
            var screenType = screenStackEntry.ScreenType;
            var tag = new ScreenLockTag($"Screen${screenType.Name}");

            try
            {
                _screenLocker.LockScreen(tag);

                if (!_historyStack.Remove(screenStackEntry))
                    return;

                await screenStackEntry.ScreenInfo.ScreenPresenter.CloseInternal(() => SetObjectsVisible(screenStackEntry.ScreenInfo, false));
                screenStackEntry.Close();

                await UpdateVisualStack();

                if (screenStackEntry.ScreenInfo.ScreenPresenter.Style.HasFlag(ScreenStyle.UnloadOnClose))
                    await _screenLoader.Unload(screenType);
            }
            finally
            {
                _screenLocker.UnlockScreen(tag);
            }
        }

        private async UniTask SetObjectsVisible(UIScreenInfo screen, bool visible)
        {
            if (visible)
            {
                await _screenLayerManager.RegisterScreen(screen);
            }
            else
            {
                await _screenLayerManager.UnregisterScreen(screen);
            }
        }

        private async UniTask UpdateVisualStack()
        {
            _historyStack.UpdateVisualOrder();

            await _screenLayerManager.UpdateStack();
        }

        public override string ToString() =>
            _historyStack.ToString();
    }
}