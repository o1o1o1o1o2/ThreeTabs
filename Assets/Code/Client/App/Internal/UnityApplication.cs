using System;
using System.Threading;
using Client.App.Contracts;
using Client.Libs.GameState;
using Client.Libs.GameState.Contracts;
using Client.Menu;
using Cysharp.Threading.Tasks;

namespace Client.App.Internal
{
    internal sealed class UnityApplication : IUnityApplication
    {
        private readonly IGameStateMachine _gameStateMachine;

        public UnityApplication(IGameStateMachine gameStateMachine) =>
            _gameStateMachine = gameStateMachine ?? throw new ArgumentNullException(nameof(gameStateMachine));

        public async UniTask MainLoop(CancellationToken ct)
        {
            await _gameStateMachine.EnterAsync<MenuState>(ct);

            try
            {
                await UniTask.WaitUntilCanceled(ct);
            }
            finally
            {
                await _gameStateMachine.EnterAsync<EmptyGameState>(CancellationToken.None);
            }
        }

        public async UniTask Reset()
        {
            await _gameStateMachine.EnterAsync<EmptyGameState>(CancellationToken.None);
            await _gameStateMachine.EnterAsync<MenuState>(CancellationToken.None);
        }
    }
}