using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Client.Libs.Networking
{
    public interface IQueuedRequestRunner : IDisposable
    {
        IQueuedRequestHandle<T> Enqueue<T>(Func<CancellationToken, UniTask<T>> executor, string debugName = null);
    }

    public interface IQueuedRequestHandle<T>
    {
        string DebugName { get; }
        bool IsStarted { get; }
        bool IsCanceled { get; }
        UniTask<T> Task { get; }
        void Cancel();
    }
}