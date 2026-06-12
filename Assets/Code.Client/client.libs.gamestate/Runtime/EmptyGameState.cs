using System.Threading;
using Client.Libs.GameState.Contracts;
using Cysharp.Threading.Tasks;

namespace Client.Libs.GameState
{
    public sealed class EmptyGameState : IGameState
    {
        public UniTask OnEnterAsync(CancellationToken ct) => UniTask.CompletedTask;
        public UniTask OnExitAsync(CancellationToken ct) => UniTask.CompletedTask;
    }
}