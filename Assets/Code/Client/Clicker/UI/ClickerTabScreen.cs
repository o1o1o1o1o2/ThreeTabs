using System;
using System.Threading;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;

namespace Client.Clicker.UI
{
    [ScreenView(typeof(ClickerTabScreenView))]
    internal class ClickerTabScreen : UIScreenPresenter
    {
        private readonly ClickerTabScreenView _screenView;
        private readonly ClickerController _clickerController;
        private CancellationTokenSource _cts;

        public ClickerTabScreen(
            ClickerTabScreenView screenView,
            IScreenManager screenManager,
            ClickerController clickerController)
            : base(screenView, screenManager)
        {
            _screenView = screenView;
            _clickerController = clickerController;
        }

        public override ScreenStyle Style => ScreenStyle.Default;

        protected override UniTask OnOpenAsync()
        {
            _cts = new CancellationTokenSource();
            _screenView.CollectClicked += OnCollectClicked;
            _clickerController.Changed += RefreshClicker;

            RefreshClicker();
            AutoCollectLoop(_cts.Token).Forget();
            EnergyRestoreLoop(_cts.Token).Forget();

            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseAsync()
        {
            _screenView.CollectClicked -= OnCollectClicked;
            _clickerController.Changed -= RefreshClicker;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            return UniTask.CompletedTask;
        }

        private void OnCollectClicked()
        {
            if (!_clickerController.TryCollect())
                return;

            _screenView.PlayCollectFx();
        }

        private void RefreshClicker() =>
            _screenView.SetClickerState(_clickerController.Currency, _clickerController.Energy, _clickerController.MaxEnergy);

        private async UniTaskVoid AutoCollectLoop(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_clickerController.AutoCollectSeconds), cancellationToken: ct);
                    OnCollectClicked();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTaskVoid EnergyRestoreLoop(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_clickerController.EnergyRestoreSeconds), cancellationToken: ct);
                    _clickerController.RestoreEnergy();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}