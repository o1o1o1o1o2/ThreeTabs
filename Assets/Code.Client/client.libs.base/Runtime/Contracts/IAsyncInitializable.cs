using System.Threading;
using Cysharp.Threading.Tasks;

namespace Client.Libs.Contracts
{
    /// <summary>
    ///     initialize class right after root classes initialized
    /// </summary>
    public interface IAsyncInitializable
    {
        UniTask InitializeAsync(CancellationToken ct);
    }
}