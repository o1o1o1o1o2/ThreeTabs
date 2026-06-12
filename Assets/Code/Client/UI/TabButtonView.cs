using System;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI
{
    [RequireComponent(typeof(Button))]
    internal class TabButtonView : MonoBehaviour
    {
        [SerializeField] private TabId _tabId;
        [SerializeField] private GameObject _activeStateRoot;
        [SerializeField] private GameObject _inactiveStateRoot;

        private Button _button;

        public event Action<TabId> Clicked;
        public TabId TabId => _tabId;

        private void Awake() =>
            _button = GetComponent<Button>();

        private void OnEnable() =>
            _button.onClick.AddListener(OnClick);

        private void OnDisable() =>
            _button.onClick.RemoveListener(OnClick);

        public void SetActive(bool isActive)
        {
            _activeStateRoot.SetActive(isActive);
            _inactiveStateRoot.SetActive(!isActive);
        }

        private void OnClick() =>
            Clicked?.Invoke(_tabId);
    }
}