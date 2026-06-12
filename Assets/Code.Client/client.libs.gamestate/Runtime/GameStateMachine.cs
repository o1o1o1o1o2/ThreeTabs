using System;
using System.Threading;
using Client.Libs.GameState.Contracts;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Client.Libs.GameState
{
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly DiContainer _container;
        private IGameState _currentState;

        public GameStateMachine(DiContainer container) =>
            _container = container ?? throw new ArgumentNullException(nameof(container));

        public Type CurrentStateType => _currentState?.GetType();
        public bool ChangingState { get; private set; }

        public bool IsActive<TState>() where TState : IGameState =>
            _currentState is TState;

        public async UniTask EnterAsync<TState>(CancellationToken ct) where TState : IGameState
        {
            if (_currentState is TState)
                return;

            ChangingState = true;

            try
            {
                if (_currentState != null)
                    await _currentState.OnExitAsync(ct);

                ct.ThrowIfCancellationRequested();

                _currentState = _container.Resolve<TState>();
                await _currentState.OnEnterAsync(ct);
            }
            finally
            {
                ChangingState = false;
            }
        }
    }
}