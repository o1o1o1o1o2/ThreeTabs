using System;
using System.Collections.Generic;
using System.Threading;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Types;
using Client.Libs.UI.Views;
using Cysharp.Threading.Tasks;

namespace Client.Libs.UI.Internal
{
    internal sealed class ScreenLocker : IScreenLocker, IDisposable
    {
        private readonly HashSet<ScreenLockTag> _lockers = new();
        private readonly UIScreenLockView _view;
        private CancellationTokenSource _delayCts;

        public ScreenLocker(UIScreenLockView view)
        {
            _view = view;
            _view.gameObject.SetActive(true);
            _view.SetLockerVisible(false);
        }

        private bool IsLocked => _lockers.Count > 0;

        public void Dispose() => CancelDelay();

        public void LockScreen(ScreenLockTag tag)
        {
            _lockers.Add(tag);
            UpdateView();
        }

        public void UnlockScreen(ScreenLockTag tag)
        {
            _lockers.Remove(tag);
            UpdateView();
        }

        private void UpdateView()
        {
            if (!_view)
                return;

            CancelDelay();

            if (IsLocked)
            {
                _view.SetLockerVisible(true);
            }
            else
            {
                _delayCts = new CancellationTokenSource();
                HideWithDelay(_delayCts.Token).Forget();
            }
        }

        private async UniTaskVoid HideWithDelay(CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(100, cancellationToken: ct);
                if (_view)
                    _view.SetLockerVisible(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CancelDelay()
        {
            _delayCts?.Cancel();
            _delayCts?.Dispose();
            _delayCts = null;
        }
    }
}