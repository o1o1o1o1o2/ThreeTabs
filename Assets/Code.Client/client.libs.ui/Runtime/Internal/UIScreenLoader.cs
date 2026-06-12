using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Client.Libs.Contracts;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Client.Libs.UI.Internal
{
    internal sealed class UIScreenLoader : IDisposable, IContainerListener
    {
        private DiContainer Container => _containers.Last();
        private readonly List<DiContainer> _containers = new(4);

        private readonly SemaphoreSlim _loadingLock = new(1, 1);

        private Dictionary<Type, UIScreenInfo> LoadedScreens { get; } = new(16);

        public void Dispose() =>
            _loadingLock?.Dispose();

        public async UniTask<UIScreenInfo> GetOrLoadScreenView(IScreenManager screenManager, Type screenType)
        {
            if (!TryResolveViewType(screenType, out var viewType))
                return null;

            await LoadIfNotLoaded(viewType);
            var screenInfo = LoadedScreens.GetValueOrDefault(viewType);
            if (screenInfo.ScreenPresenter != null)
                return screenInfo;

            var presenter = (UIScreenPresenter)Container.Instantiate(screenType, new object[] { screenInfo.View, screenManager });
            screenInfo.WithPresenter(presenter);

            return screenInfo;
        }

        private async UniTask LoadIfNotLoaded(Type viewType)
        {
            if (LoadedScreens.ContainsKey(viewType))
                return;

            await _loadingLock.WaitAsync();
            try
            {
                if (LoadedScreens.ContainsKey(viewType))
                    return;

                var sceneName = GetScreenParentSceneName(viewType);
                var scene = SceneManager.GetSceneByName(sceneName);

                if (!scene.IsValid() || !scene.isLoaded)
                {
                    Debug.Log($"Load built-in screen: {sceneName}");
                    await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    scene = SceneManager.GetSceneByName(sceneName);
                }

                TryRemoveAllUnwantedGos(scene);

                var (view, cameras) = UnpackScene(scene, viewType);
                if (view == null)
                {
                    Debug.LogError($"There is no screen of type:{viewType.Name} in scene:{sceneName}");
                    return;
                }

                if (LoadedScreens.TryGetValue(viewType, out var oldScreen))
                {
                    Debug.LogError($"Scene with name:{sceneName} contains screen with the same type:{viewType} already loaded in scene {scene.name}");
                    return;
                }

                var screenInfo = new UIScreenInfo(view, scene, cameras);
                LoadedScreens.Add(viewType, screenInfo);

                InjectToWidgets(scene);
            }
            finally
            {
                _loadingLock.Release();
            }
        }

        public async UniTask Unload(Type screenType)
        {
            if (!LoadedScreens.ContainsKey(screenType))
                return;

            await _loadingLock.WaitAsync();

            try
            {
                if (!LoadedScreens.TryGetValue(screenType, out var screenContainerInfo))
                    return;

                await SceneManager.UnloadSceneAsync(screenContainerInfo.Scene);
                LoadedScreens.Remove(screenType);
            }
            finally
            {
                _loadingLock.Release();
            }
        }

        public async UniTask UnloadAll(IList<Type> exceptList = null)
        {
            foreach (var type in LoadedScreens.Keys.Where(x => !exceptList?.Contains(x) ?? true).ToArray())
                await Unload(type);
        }

        private void InjectToWidgets(Scene scene)
        {
            var container = Container;
            foreach (var parentGo in scene.GetRootGameObjects())
                container.InjectGameObject(parentGo);
        }

        private static void TryRemoveAllUnwantedGos(Scene scene)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects().ToArray())
            {
                TryRemoveAllUnwantedGosOfType<Light>(rootGameObject);
                TryRemoveAllUnwantedGosOfType<EventSystem>(rootGameObject);
            }
        }

        private static void TryRemoveAllUnwantedGosOfType<T>(GameObject parent) where T : Component
        {
            if (parent == null)
                return;

            var gameObjectsToRemove = parent.GetComponentsInChildren<T>(true).Select(x => x.gameObject).Distinct().ToArray();
            foreach (var gameObject in gameObjectsToRemove)
                Object.DestroyImmediate(gameObject);
        }

        private static (UIScreenView, Camera[]) UnpackScene(Scene scene, Type viewType)
        {
            var cameras = new List<Camera>(1);
            UIScreenView view = null;

            foreach (var gObject in scene.GetRootGameObjects().Where(x => (x.hideFlags & HideFlags.DontSave) == 0))
            {
                foreach (var objectView in gObject.GetComponentsInChildren<UIScreenView>(true))
                {
                    if (!view)
                        view = objectView;
                    else if (view != objectView)
                        Debug.LogError($"Only one screen view supported. Duplicated type:{objectView.GetType().Name} scene:{scene.name}");

                    if (objectView.GetType() != viewType)
                        Debug.LogError($"Unexpected screen view type '{objectView.GetType().Name}' in scene:{scene.name}; expected '{viewType.Name}'");
                }

                cameras.AddRange(gObject.GetComponentsInChildren<Camera>(true));
            }

            if (!view)
            {
                UILogger.LogError($"There is no screen view in scene:{scene.name}");
                return (null, Array.Empty<Camera>());
            }

            var viewCamera = view.GetRootCanvas().rootCanvas.worldCamera;
            if (viewCamera != null)
                cameras.Add(viewCamera);

            return (view, cameras.Where(x => x != null).Distinct().ToArray());
        }

        private static string GetScreenParentSceneName(Type screenType)
        {
            var sceneNameAttr = screenType.GetCustomAttribute<ScreenParentSceneNameAttribute>();
            if (sceneNameAttr != null)
                return sceneNameAttr.SceneName;

            var typeName = screenType.Name;
            const string suffix = "View";
            return typeName.EndsWith(suffix) ? typeName.Substring(0, typeName.Length - suffix.Length) : typeName;
        }

        public void OnInstall(DiContainer container)
        {
            if (_containers.Contains(container))
                return;

            _containers.Add(container);
        }

        public void OnUninstall(DiContainer container) =>
            _containers.Remove(container);

        private static bool TryResolveViewType(Type presenterType, out Type viewType)
        {
            var attribute = presenterType.GetCustomAttribute<ScreenViewAttribute>();
            if (attribute != null)
            {
                viewType = attribute.ViewType;
                return true;
            }

            viewType = null;
            Debug.LogError($"Presenter type '{presenterType.Name}' must be marked with {nameof(ScreenViewAttribute)}.");
            return false;
        }
    }
}