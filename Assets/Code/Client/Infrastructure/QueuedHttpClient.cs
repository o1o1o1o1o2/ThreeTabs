using System;
using System.Threading;
using Client.Libs.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Client.Infrastructure
{
    internal sealed class QueuedHttpClient
    {
        private readonly IQueuedRequestRunner _requestRunner;

        public QueuedHttpClient(IQueuedRequestRunner requestRunner) =>
            _requestRunner = requestRunner ?? throw new ArgumentNullException(nameof(requestRunner));

        public IQueuedRequestHandle<string> GetText(string url, string debugName) =>
            _requestRunner.Enqueue(ct => SendGet(url, ct), debugName);

        public IQueuedRequestHandle<T> Enqueue<T>(Func<CancellationToken, UniTask<T>> executor, string debugName) =>
            _requestRunner.Enqueue(executor, debugName);

        private static async UniTask<string> SendGet(string url, CancellationToken ct)
        {
            using var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Accept", "application/json");

            await request.SendWebRequest().ToUniTask(cancellationToken: ct);

            return request.result != UnityWebRequest.Result.Success
                ? throw new InvalidOperationException($"{request.responseCode}: {request.error}")
                : request.downloadHandler.text;
        }
    }
}