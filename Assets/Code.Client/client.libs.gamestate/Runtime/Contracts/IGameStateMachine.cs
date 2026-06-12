using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Client.Libs.GameState.Contracts
{
    public interface IGameStateMachine
    {
        Type CurrentStateType { get; }
        bool ChangingState { get; }
        bool IsActive<TState>() where TState : IGameState;
        UniTask EnterAsync<TState>(CancellationToken ct) where TState : IGameState;
    }
}