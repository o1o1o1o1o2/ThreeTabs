using System.Threading;
using Cysharp.Threading.Tasks;

namespace Client.App.Contracts
{
    public interface IUnityApplication
    {
        UniTask MainLoop(CancellationToken ct);
        UniTask Reset();
    }
}