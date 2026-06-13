using System.Linq;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Libs.UI.Internal
{
    internal class UIScreenInfo
    {
        private const int ALWAYS_ON_TOP_PRIORITY_OFFSET = 100000;

        public int VisualOrder { get; set; }

        public UIScreenPresenter ScreenPresenter { get; private set; }
        public UIScreenView View { get; }
        public Scene Scene { get; }

        public Camera[] Cameras { get; }

        public bool IsFullscreen => ScreenPresenter != null && ScreenPresenter.Style.HasFlag(ScreenStyle.FullScreen);
        public bool IsAlwaysOnTop => ScreenPresenter != null && ScreenPresenter.Style.HasFlag(ScreenStyle.AlwaysOnTop);
        public int Priority => IsAlwaysOnTop ? ALWAYS_ON_TOP_PRIORITY_OFFSET + VisualOrder : VisualOrder;

        private readonly UIScreenView _view;

        public UIScreenInfo(UIScreenView view, Scene scene, Camera[] cameras)
        {
            View = view;
            Scene = scene;
            _view = view;

            Cameras = cameras.Where(x => x != null).Distinct().ToArray();
            foreach (var camera in Cameras)
                camera.enabled = false;
        }

        public UniTask VisibleChanged(bool visible)
        {
            if (_view)
                _view.CamerasVisibleChanged(visible);
            return UniTask.CompletedTask;
        }

        public void WithPresenter(UIScreenPresenter screenPresenter) =>
            ScreenPresenter = screenPresenter;

        public override string ToString() => $"Screen '{(ScreenPresenter != null ? ScreenPresenter.GetType().Name : View.GetType().Name)}'";
    }
}