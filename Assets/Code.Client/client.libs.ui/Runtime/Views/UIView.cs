using System;
using Client.Libs.UI.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Client.Libs.UI.Views
{
    /// <summary>
    ///     base class for all UI views
    /// </summary>
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster)),
     RequireComponent(typeof(CanvasGroup))]
    public abstract partial class UIView : MonoBehaviour
    {
        private RectTransform _rectTm;
        private int _locks;
        private protected IScreenLocker ScreenLockerInternal;
        private IScreenManager _screenManager;
        protected RectTransform RectTransform => _rectTm != null ? _rectTm : _rectTm = (RectTransform)transform;

        private CanvasGroup _canvasGroupCached;
        private CanvasGroup CanvasGroup => _canvasGroupCached ??= GetComponent<CanvasGroup>();
        private Canvas _canvas;
        private Canvas Canvas => _canvas ??= this.GetRootCanvas();
        private Camera _camera;
        private Camera Camera => _camera ??= Canvas.worldCamera;
        protected bool IsVisible => gameObject.activeSelf && CanvasGroup.alpha != 0 && (Camera == null || Camera.enabled);

        protected virtual void OnDestroy() =>
            _screenManager?.Unregister(this);

        protected virtual void InitializeView()
        {
        }

        public event Action<UIView> BeforeShown;
        public event Action<UIView> OnHidden;

        [Inject]
        private void Construct(IScreenManager screenManager, IScreenLocker screenLocker)
        {
            _screenManager = screenManager;
            ScreenLockerInternal = screenLocker;
            Initialize();
        }

        private void Initialize()
        {
            _screenManager.Register(this);
            InitializeView();
            gameObject.SetActive(false);
        }

        private protected UniTask ShowViewAsync(Func<UniTask> setVisible)
        {
#if VIEW_LOGS
			UILogger.Log($"[{GetType().Name}] Show");
#endif

            return ShowInternalAsync(setVisible);
        }

        private protected UniTask HideViewAsync()
        {
#if VIEW_LOGS
			UILogger.Log($"[{GetType().Name}] Hide");
#endif

            return HideInternalAsync();
        }

        private protected void PreWarm()
        {
            CanvasGroup.alpha = 0.0f;
            gameObject.SetActive(true);
        }

        private async UniTask ShowInternalAsync(Func<UniTask> setVisible)
        {
#if VIEW_LOGS
			UILogger.Log($"[{GetType().Name}] ViewOnBeforeShown");
#endif
            BeforeShown?.Invoke(this);

            await BeforeShowAsync();

            CanvasGroup.alpha = 1.0f;

            await setVisible();
        }

        private async UniTask HideInternalAsync()
        {
            OnHidden?.Invoke(this);

            gameObject.SetActive(false);

            await AfterHideAsync();
        }

        protected virtual UniTask BeforeShowAsync() => UniTask.CompletedTask;
        protected virtual UniTask AfterHideAsync() => UniTask.CompletedTask;

        public virtual void CamerasVisibleChanged(bool visible)
        {
        }
    }
}
