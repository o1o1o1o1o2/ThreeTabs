#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Client.Libs.UI.Views
{
    public abstract partial class UIView
    {
        private const float CAMERA_Z_DEPTH = 100.0f;
        private const string UI_CAM_PREFAB = "Assets/Data.Internal/Cameras/Camera - UI.prefab";

        [Button(ButtonSizes.Medium, "Add To Build")]
        public void SetEditorBuildSettingsScenes()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            var newScene = new EditorBuildSettingsScene(gameObject.scene.path, true);
            if (scenes.FirstOrDefault(x => x.path == newScene.path) != null)
                return;
            scenes.Add(newScene);
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private bool NoCamera => Canvas == null || Canvas.worldCamera == null;

        [Button(ButtonSizes.Medium, "Add Camera"), GUIColor(1, 0, 0), ShowIf(nameof(NoCamera))]
        public void InstantiateAndSetupCamera()
        {
            var camPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(UI_CAM_PREFAB);
            var cam = ((GameObject)PrefabUtility.InstantiatePrefab(camPrefab)).GetComponent<Camera>();
            cam.name = $"{name}Cam";
            cam.transform.parent = null;
            cam.nearClipPlane = -CAMERA_Z_DEPTH * 0.5f;
            cam.farClipPlane = CAMERA_Z_DEPTH * 0.5f;

            var canvas = this.GetRootCanvas().rootCanvas;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 0;

            var cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData != null)
                cameraData.renderType = CameraRenderType.Overlay;

            if (gameObject.scene.IsValid())
                EditorUtility.SetDirty(canvas);
        }
    }
}
#endif
