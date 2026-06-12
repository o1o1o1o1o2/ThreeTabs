using Client.Libs.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Client.Libs
{
    public sealed class EntryPoint : MonoBehaviour
    {
        private void Awake() =>
            StartGame().Forget();

        private static async UniTaskVoid StartGame()
        {
            await UniTask.NextFrame();

            var projectContext = ProjectContext.Instance;

            await UniTask.NextFrame();

            projectContext.Container.Resolve<IApplicationLoader>().Play().Forget();
        }
    }
}