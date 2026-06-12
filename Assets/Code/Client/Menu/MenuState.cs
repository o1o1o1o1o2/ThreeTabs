using System;
using System.Threading;
using Client.App.Configs;
using Client.Libs.GameState.Contracts;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Extensions;
using Client.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Client.Menu
{
    internal sealed class MenuState : IGameState
    {
        private readonly ISceneConfig _sceneConfig;
        private readonly IScreenManager _screenManager;
        private readonly ZenjectSceneLoader _sceneLoader;

        private bool _gameSceneLoaded;

        public MenuState(ISceneConfig sceneConfig, IScreenManager screenManager, ZenjectSceneLoader sceneLoader)
        {
            _sceneConfig = sceneConfig ?? throw new ArgumentNullException(nameof(sceneConfig));
            _screenManager = screenManager ?? throw new ArgumentNullException(nameof(screenManager));
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        }

        public async UniTask OnEnterAsync(CancellationToken ct)
        {
            if (!_gameSceneLoaded)
            {
                var operation = _sceneLoader.LoadSceneAsync(
                    _sceneConfig.MenuSceneName,
                    LoadSceneMode.Additive);

                await operation.ToUniTask(cancellationToken: ct);
                _gameSceneLoaded = true;
            }

            await _screenManager.Screen<MainTabsScreen>().Open();
        }

        public async UniTask OnExitAsync(CancellationToken ct)
        {
            await _screenManager.Screen<MainTabsScreen>().Close();

            if (!_gameSceneLoaded)
                return;

            var scene = SceneManager.GetSceneByName(_sceneConfig.MenuSceneName);
            if (scene.IsValid() && scene.isLoaded)
                await SceneManager.UnloadSceneAsync(scene).ToUniTask(cancellationToken: ct);

            _gameSceneLoaded = false;
        }
    }
}