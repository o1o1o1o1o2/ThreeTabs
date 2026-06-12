using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Client.Libs.UI.Internal;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Debug = UnityEngine.Debug;

namespace Client.Libs.UI.Cameras
{
    public sealed partial class ScreenLayerManager : MonoBehaviour
    {
        [SerializeField] private Camera _baseCamera;

        private readonly List<UIScreenInfo> _screens = new(8);
        private readonly SemaphoreSlim _updateLock = new(1, 1);
        private UniversalAdditionalCameraData _baseCameraData;

        private Camera[] AllCameras { get; set; } = Array.Empty<Camera>();

        private void Awake()
        {
            Debug.Assert(_baseCamera != null, $"{nameof(ScreenLayerManager)} requires base camera.");
            _baseCameraData = _baseCamera.GetComponent<UniversalAdditionalCameraData>();
            Debug.Assert(_baseCameraData != null, $"{nameof(ScreenLayerManager)} base camera requires {nameof(UniversalAdditionalCameraData)}.");
        }

        private IEnumerable<UIScreenInfo> GetScreens() =>
            _screens.OrderBy(x => x.Priority);

        internal UniTask RegisterScreen(UIScreenInfo screen)
        {
            if (screen == null)
                return UniTask.CompletedTask;

            foreach (var cam in screen.Cameras)
            {
                if (cam)
                    cam.enabled = false;
            }

            if (_screens.Contains(screen))
                return UniTask.CompletedTask;

            _screens.Add(screen);
            return UpdateStack();
        }

        internal async UniTask UnregisterScreen(UIScreenInfo screen)
        {
            if (screen == null)
                return;

            await DisableScreen(screen);

            if (_screens.Remove(screen))
                await UpdateStack();
        }

        [Conditional("DEBUG_UI_SCREENS")]
        private static void DebugLog(string message) => Debug.Log($"[CameraLayerManager] {message}");

        internal async UniTask UpdateStack()
        {
            if (_baseCameraData == null)
                return;

            await _updateLock.WaitAsync();
            try
            {
                var screens = GetScreens().ToArray();
                var cameras = screens.SelectMany(x => x.Cameras).Where(x => x != null).ToArray();

                RemoveUnusedCameras(cameras);

                DebugLog($"UpdateStack cameras: {string.Join(", ", cameras.Select(x => x.name))}");

                var fullscreenScreen = screens.LastOrDefault(x => x.IsFullscreen);
                var canEnable = fullscreenScreen == null;
                var depth = 1;

                foreach (var screen in screens)
                {
                    if (!canEnable && screen != fullscreenScreen && !screen.IsAlwaysOnTop)
                    {
                        await DisableScreen(screen);
                        continue;
                    }

                    if (screen == fullscreenScreen)
                        canEnable = true;

                    depth = await EnableScreen(screen, depth);
                }

                AllCameras = cameras;

#if UNITY_EDITOR
                UpdateDebugInfo();
#endif
            }
            finally
            {
                _updateLock.Release();
            }
        }

        private void RemoveUnusedCameras(IReadOnlyCollection<Camera> activeCameras)
        {
            for (var i = _baseCameraData.cameraStack.Count - 1; i >= 0; i--)
            {
                var cam = _baseCameraData.cameraStack[i];
                if (!activeCameras.Contains(cam))
                    _baseCameraData.cameraStack.RemoveAt(i);
            }
        }

        private async UniTask<int> EnableScreen(UIScreenInfo screen, int depth)
        {
            foreach (var cam in screen.Cameras)
            {
                if (!cam)
                    continue;

                var cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
                if (cameraData == null)
                {
                    Debug.LogError($"Camera '{cam.name}' requires {nameof(UniversalAdditionalCameraData)}.");
                    continue;
                }

                cam.depth = ++depth;
                cameraData.renderType = CameraRenderType.Overlay;

                _baseCameraData.cameraStack.Remove(cam);
                _baseCameraData.cameraStack.Add(cam);
                cam.enabled = true;
            }

            await screen.VisibleChanged(true);
            return depth;
        }

        private UniTask DisableScreen(UIScreenInfo screen) =>
            DisableScreenInternal(_baseCameraData, screen);

        private static async UniTask DisableScreenInternal(UniversalAdditionalCameraData baseCameraData, UIScreenInfo screen)
        {
            await screen.VisibleChanged(false);

            foreach (var camera in screen.Cameras)
            {
                if (!camera)
                    continue;

                baseCameraData.cameraStack.Remove(camera);
                camera.enabled = false;
            }
        }
    }
}