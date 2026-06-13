using System;
using Client.Clicker.UI;
using Client.Dogs.UI;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Extensions;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Client.Weather.UI;
using Cysharp.Threading.Tasks;

namespace Client.UI
{
    [ScreenView(typeof(MainTabsScreenView))]
    internal class MainTabsScreen : UIScreenPresenter
    {
        private readonly MainTabsScreenView _view;
        private readonly IScreenManager _screenManager;
        private Type _activeTabScreenType;
        private TabId? _pendingTabId;
        private bool _isSwitchingTab;
        private bool _isClosing;

        public MainTabsScreen(MainTabsScreenView view, IScreenManager screenManager) : base(view, screenManager)
        {
            _view = view;
            _screenManager = screenManager;
        }

        public override ScreenStyle Style => ScreenStyle.AlwaysOnTop;

        protected override UniTask OnOpenAsync()
        {
            _isClosing = false;
            _view.TabSelected += OnTabSelected;
            OnTabSelected(TabId.Clicker);
            return UniTask.CompletedTask;
        }

        protected override async UniTask OnCloseAsync()
        {
            _isClosing = true;
            _pendingTabId = null;
            _view.TabSelected -= OnTabSelected;

            while (_isSwitchingTab)
                await UniTask.NextFrame();

            if (_activeTabScreenType != null)
            {
                await _screenManager.CloseLast(_activeTabScreenType);
                _activeTabScreenType = null;
            }
        }

        private void OnTabSelected(TabId tabId) =>
            RequestSwitchTab(tabId).Forget();

        private async UniTaskVoid RequestSwitchTab(TabId tabId)
        {
            if (_isClosing)
                return;

            _pendingTabId = tabId;

            if (_isSwitchingTab)
                return;

            _isSwitchingTab = true;
            try
            {
                while (_pendingTabId.HasValue && !_isClosing)
                {
                    var nextTabId = _pendingTabId.Value;
                    _pendingTabId = null;
                    await SwitchTab(nextTabId);
                }
            }
            finally
            {
                _isSwitchingTab = false;
            }
        }

        private async UniTask SwitchTab(TabId tabId)
        {
            if (_isClosing)
                return;

            _view.SetActiveTab(tabId);
            var nextTabScreenType = GetTabScreenType(tabId);

            if (_activeTabScreenType == nextTabScreenType)
                return;

            if (_activeTabScreenType != null)
                await _screenManager.CloseLast(_activeTabScreenType);

            if (_isClosing)
                return;

            _activeTabScreenType = nextTabScreenType;
            await OpenTab(tabId);
        }

        private static Type GetTabScreenType(TabId tabId) =>
            tabId switch
            {
                TabId.Clicker => typeof(ClickerTabScreen),
                TabId.Weather => typeof(WeatherTabScreenPresenter),
                TabId.Dogs => typeof(DogsTabScreen),
                _ => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
            };

        private UniTask OpenTab(TabId tabId) =>
            tabId switch
            {
                TabId.Clicker => _screenManager.Screen<ClickerTabScreen>().Open(),
                TabId.Weather => _screenManager.Screen<WeatherTabScreenPresenter>().Open(),
                TabId.Dogs => _screenManager.Screen<DogsTabScreen>().Open(),
                _ => throw new ArgumentOutOfRangeException(nameof(tabId), tabId, null)
            };
    }
}
