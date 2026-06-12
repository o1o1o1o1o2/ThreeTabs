using System;
using Client.Libs.UI.Internal;
using UnityEngine;

namespace Client.UI
{
    internal sealed class MainTabsScreenView : UIScreenView
    {
        [SerializeField] private TabButtonView[] _tabs;

        public event Action<TabId> TabSelected;

        protected override void InitializeView()
        {
            base.InitializeView();

            foreach (var tab in _tabs)
            {
                tab.Clicked += OnTabClicked;
            }
        }

        protected override void OnDestroy()
        {
            foreach (var tab in _tabs)
            {
                if (tab != null)
                    tab.Clicked -= OnTabClicked;
            }

            base.OnDestroy();
        }

        public void SetActiveTab(TabId activeTabId)
        {
            foreach (var tab in _tabs)
            {
                tab.SetActive(tab.TabId == activeTabId);
            }
        }

        private void OnTabClicked(TabId tabId) =>
            TabSelected?.Invoke(tabId);
    }
}