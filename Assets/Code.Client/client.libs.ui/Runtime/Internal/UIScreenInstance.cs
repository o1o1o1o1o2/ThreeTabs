using System.Linq;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client.Libs.UI.Internal
{
    internal class UIScreenInfo
    {
        public int VisualOrder { get; set; }

        public UIScreenPresenter ScreenPresenter { get; private set; }
        public UIScreenView View { get; }
        public Scene Scene { get; }

        public Camera[] Cameras { get; }

        public bool IsFullscreen => ScreenPresenter != null && ScreenPresenter.Style.HasFlag(ScreenStyle.FullScreen);
        public bool IsAlwaysOnTop => ScreenPresenter != null && ScreenPresenter.Style.HasFlag(ScreenStyle.AlwaysOnTop);
        public int Priority => IsAlwaysOnTop ? int.MaxValue : VisualOrder;

        private readonly GraphicRaycaster[] _raycasters;
        private readonly CanvasGroup[] _groups;
        private readonly UIScreenView _view;

        public UIScreenInfo(UIScreenView view, Scene scene, Camera[] cameras)
        {
            View = view;
            Scene = scene;
            _view = view;

            var roots = scene.GetRootGameObjects();

            _raycasters = roots
                .SelectMany(x => x.GetComponentsInChildren<GraphicRaycaster>(true))
                .Distinct()
                .ToArray();

            _groups = roots
                .SelectMany(x => x.GetComponentsInChildren<CanvasGroup>(true))
                .Distinct()
                .ToArray();

            Cameras = cameras.Where(x => x != null).Distinct().ToArray();
            foreach (var camera in Cameras)
                camera.enabled = false;
        }

        public UniTask VisibleChanged(bool visible)
        {
            foreach (var raycaster in _raycasters)
                if (raycaster)
                    raycaster.enabled = visible;

            foreach (var x in _groups)
            {
                if (x)
                    x.blocksRaycasts = visible;
            }

            if (_view)
                _view.CamerasVisibleChanged(visible);
            return UniTask.CompletedTask;
        }

        public void WithPresenter(UIScreenPresenter screenPresenter) =>
            ScreenPresenter = screenPresenter;

        public override string ToString() => $"Screen '{(ScreenPresenter != null ? ScreenPresenter.GetType().Name : View.GetType().Name)}'";
    }
}
