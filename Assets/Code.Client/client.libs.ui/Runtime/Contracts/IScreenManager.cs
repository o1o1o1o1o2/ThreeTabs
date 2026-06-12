using System;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Views;
using Cysharp.Threading.Tasks;

namespace Client.Libs.UI.Contracts
{
    public interface IScreenManager
    {
        void Register(UIView view);
        void Unregister(UIView view);

        internal UniTask<UIScreenEntry> Show(Type screenType);
        UniTask CloseLast(Type screenType);
    }
}
