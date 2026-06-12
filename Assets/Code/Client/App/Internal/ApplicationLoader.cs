using System;
using System.Linq;
using System.Threading;
using Client.App.Configs;
using Client.App.Contracts;
using Client.Libs.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Client.App.Internal
{
    internal sealed class ApplicationLoader : IApplicationLoader
    {
        private readonly ISceneConfig _sceneConfig;
        private readonly ZenjectSceneLoader _sceneLoader;

        public ApplicationLoader(ISceneConfig sceneConfig, ZenjectSceneLoader sceneLoader)
        {
            _sceneConfig = sceneConfig ?? throw new ArgumentNullException(nameof(sceneConfig));
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        }

        public async UniTask Play()
        {
            var operation = _sceneLoader.LoadSceneAsync(_sceneConfig.MainSceneName);
            await operation.ToUniTask();
            await UniTask.NextFrame();

            var application = ResolveFromScene<IUnityApplication>(_sceneConfig.MainSceneName);
            await application.MainLoop(CancellationToken.None);
        }

        private static T ResolveFromScene<T>(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            var sceneContext = scene.GetRootGameObjects()
                .Select(root => root.GetComponentInChildren<SceneContext>())
                .FirstOrDefault(context => context != null);

            return sceneContext == null ? throw new InvalidOperationException($"Scene '{sceneName}' does not contain {nameof(SceneContext)}.") : sceneContext.Container.Resolve<T>();
        }
    }
}