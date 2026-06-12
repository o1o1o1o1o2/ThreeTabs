#if UNITY_EDITOR

using System.Linq;
using Client.Libs.UI.Internal;
using UnityEngine;

namespace Client.Libs.UI.Cameras {

	public sealed partial class ScreenLayerManager {
		[SerializeField, TextArea(10, 20)] private string _stack;
		[SerializeField] private Camera[] _cameras;

		private void UpdateDebugInfo() {
			_stack = string.Join("\n", GetScreens().Select(GetDesc));
			_cameras = _baseCameraData.cameraStack.ToArray();
		}

		private static string GetName(UIScreenInfo screen) {
			if (screen == null)
				return "[<null>]";

			var fullscreen = screen.IsFullscreen ? " <FULLSCREEN>" : string.Empty;
			var camera = screen.Cameras.FirstOrDefault();
			return camera
				? $"[screen={screen} cam={camera.name} @{screen.Priority}{fullscreen}]"
				: $"[{screen} @{screen.Priority}{fullscreen}]";
		}

		private static string GetDesc(UIScreenInfo screen) {
			if (screen == null)
				return string.Empty;

			var off = screen.Cameras.Any(x => x && x.enabled) ? string.Empty : "<OFF> ";
			return $"{off}{GetName(screen)}";
		}
	}
}

#endif
