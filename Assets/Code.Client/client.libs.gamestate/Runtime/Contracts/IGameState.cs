using System.Threading;
using Cysharp.Threading.Tasks;

namespace Client.Libs.GameState.Contracts
{
    public interface IGameState
    {
        UniTask OnEnterAsync(CancellationToken ct);
        UniTask OnExitAsync(CancellationToken ct);
    }
}