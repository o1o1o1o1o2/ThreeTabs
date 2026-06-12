using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Client.Libs.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Client.Libs.Extensions
{
    public static class AsyncInitializableHelper
    {
        private static readonly List<IAsyncInitializable> ReadyItems = new(32);
        private static readonly List<IAsyncInitializable> PendingItems = new(32);

        public static void QueueAsyncInitializers(this DiContainer container)
        {
            foreach (var item in container.ResolveAll<IAsyncInitializable>()
                         .Where(item => !ReadyItems.Contains(item)))
                PendingItems.Add(item);
        }

        public static async UniTask InitializeAsync(CancellationToken ct = default)
        {
            var items = PendingItems.ToArray();
            PendingItems.Clear();
            ReadyItems.AddRange(items);

            await UniTask.WhenAll(items
                .Select(item =>
                {
                    Debug.Log($"InitializeAsync: {item.GetType().Name}");
                    return item.InitializeAsync(ct);
                }));
        }
    }
}