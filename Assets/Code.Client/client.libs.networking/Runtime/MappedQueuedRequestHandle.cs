using System;
using Cysharp.Threading.Tasks;

namespace Client.Libs.Networking
{
    public sealed class MappedQueuedRequestHandle<TSource, TResult> : IQueuedRequestHandle<TResult>
    {
        private readonly IQueuedRequestHandle<TSource> _source;
        private readonly Func<TSource, TResult> _map;

        public MappedQueuedRequestHandle(IQueuedRequestHandle<TSource> source, Func<TSource, TResult> map)
        {
            _source = source;
            _map = map;
        }

        public string DebugName => _source.DebugName;
        public bool IsStarted => _source.IsStarted;
        public bool IsCanceled => _source.IsCanceled;
        public UniTask<TResult> Task => AwaitMapped();

        public void Cancel() => _source.Cancel();

        private async UniTask<TResult> AwaitMapped()
        {
            var source = await _source.Task;
            return _map.Invoke(source);
        }
    }
}