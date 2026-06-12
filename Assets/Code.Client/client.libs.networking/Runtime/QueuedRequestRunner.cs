using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Client.Libs.Networking
{
    public sealed class QueuedRequestRunner : IQueuedRequestRunner
    {
        private readonly Queue<IQueuedRequest> _queue = new();
        private readonly object _sync = new();
        private IQueuedRequest _activeRequest;
        private bool _isProcessing;
        private bool _isDisposed;

        public IQueuedRequestHandle<T> Enqueue<T>(Func<CancellationToken, UniTask<T>> executor, string debugName = null)
        {
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));

            var request = new QueuedRequest<T>(executor, debugName, RemoveQueued);

            lock (_sync)
            {
                ThrowIfDisposed();
                _queue.Enqueue(request);

                if (_isProcessing)
                    return request;
                _isProcessing = true;
                ProcessQueueAsync().Forget();
            }

            return request;
        }

        public void Dispose()
        {
            IQueuedRequest[] pending;

            lock (_sync)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                pending = _queue.ToArray();
                _queue.Clear();
            }

            _activeRequest?.Cancel();

            foreach (var request in pending)
                request.Cancel();
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            while (true)
            {
                IQueuedRequest request;

                lock (_sync)
                {
                    if (_isDisposed || _queue.Count == 0)
                    {
                        _isProcessing = false;
                        return;
                    }

                    request = _queue.Dequeue();
                    _activeRequest = request;
                }

                if (request.IsCanceled)
                    continue;

                await request.ExecuteAsync();

                lock (_sync)
                {
                    if (ReferenceEquals(_activeRequest, request))
                        _activeRequest = null;
                }
            }
        }

        private void RemoveQueued(IQueuedRequest request)
        {
            lock (_sync)
            {
                if (_queue.Count == 0)
                    return;

                var kept = new Queue<IQueuedRequest>(_queue.Count);

                while (_queue.Count > 0)
                {
                    var queued = _queue.Dequeue();
                    if (!ReferenceEquals(queued, request))
                        kept.Enqueue(queued);
                }

                while (kept.Count > 0)
                    _queue.Enqueue(kept.Dequeue());
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(QueuedRequestRunner));
        }

        private interface IQueuedRequest
        {
            bool IsCanceled { get; }
            void Cancel();
            UniTask ExecuteAsync();
        }

        private sealed class QueuedRequest<T> : IQueuedRequest, IQueuedRequestHandle<T>
        {
            private readonly Func<CancellationToken, UniTask<T>> _executor;
            private readonly Action<IQueuedRequest> _removeQueued;
            private readonly CancellationTokenSource _cts = new();
            private readonly UniTaskCompletionSource<T> _completion = new();

            public QueuedRequest(Func<CancellationToken, UniTask<T>> executor, string debugName, Action<IQueuedRequest> removeQueued)
            {
                _executor = executor;
                _removeQueued = removeQueued;
                DebugName = string.IsNullOrWhiteSpace(debugName) ? typeof(T).Name : debugName;
            }

            public string DebugName { get; }
            public bool IsStarted { get; private set; }
            public bool IsCanceled { get; private set; }
            public UniTask<T> Task => _completion.Task;

            public void Cancel()
            {
                if (IsCanceled)
                    return;

                IsCanceled = true;
                _cts.Cancel();

                if (IsStarted)
                    return;
                _removeQueued.Invoke(this);
                _completion.TrySetCanceled(_cts.Token);
                _cts.Dispose();
            }

            public async UniTask ExecuteAsync()
            {
                IsStarted = true;

                try
                {
                    var result = await _executor.Invoke(_cts.Token);
                    _completion.TrySetResult(result);
                }
                catch (OperationCanceledException)
                {
                    IsCanceled = true;
                    _completion.TrySetCanceled(_cts.Token);
                }
                catch (Exception exception)
                {
                    _completion.TrySetException(exception);
                }
                finally
                {
                    _cts.Dispose();
                }
            }
        }
    }
}