using System;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Internal;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Client.Libs.UI.Screens
{
    public abstract class UIScreenPresenter : IUIScreen
    {
        protected IScreenManager ScreenManager { get; private set; }
        protected UIScreenView ScreenView { get; private set; }
        protected IScreenLocker ScreenLocker => ScreenView.ScreenLocker;

        public ScreenState State { get; private set; } = ScreenState.Hidden;

        public abstract ScreenStyle Style { get; }

        protected UIScreenPresenter(UIScreenView screenView, IScreenManager screenManager)
        {
            ScreenView = screenView;
            ScreenManager = screenManager;
            Initialize();
        }

        private void Initialize()
        {
            if (ScreenView == null)
            {
                Debug.LogError($"Cannot initialize screen presenter '{GetType().Name}' without {nameof(UIScreenView)}.");
            }
        }

        internal async UniTask OpenInternal(Func<UniTask> setVisible, bool forceRefresh = false)
        {
            while (State != ScreenState.Visible && State != ScreenState.Hidden)
                await UniTask.DelayFrame(1);

            if (!forceRefresh && State == ScreenState.Visible)
                return;

            State = ScreenState.Showing;

            try
            {
                if (!forceRefresh)
                    ScreenView.PreWarmScreen();
                await OnOpenAsync();

                await ScreenView.ShowScreenAsync(setVisible);
                State = ScreenState.Visible;
            }
            catch (Exception)
            {
                State = ScreenState.Hidden;
                throw;
            }
        }

        internal async UniTask CloseInternal(Action doAfterHide = null)
        {
            while (State != ScreenState.Visible && State != ScreenState.Hidden)
                await UniTask.DelayFrame(1);

            if (State == ScreenState.Hidden)
                return;

            State = ScreenState.Hiding;

            try
            {
                await OnCloseAsync();
                doAfterHide?.Invoke();
                await ScreenView.HideScreenAsync();
                State = ScreenState.Hidden;
            }
            catch (Exception)
            {
                State = ScreenState.Visible;
                throw;
            }
        }

        public void Close() => CloseAsync().Forget();
        public UniTask CloseAsync() => ScreenManager.CloseLast(GetType());

        UniTask IUIScreen.OpenInternal(Func<UniTask> setVisible, bool forceRefresh) => OpenInternal(setVisible, forceRefresh);
        UniTask IUIScreen.CloseInternal(Action doAfterHide) => CloseInternal(doAfterHide);

        protected virtual UniTask OnOpenAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnCloseAsync() => UniTask.CompletedTask;

        public override string ToString() => GetType().Name;
    }
}