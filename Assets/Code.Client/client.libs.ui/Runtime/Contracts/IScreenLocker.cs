using Client.Libs.UI.Types;

namespace Client.Libs.UI.Contracts
{
    public interface IScreenLocker
    {
        void LockScreen(ScreenLockTag tag);
        void UnlockScreen(ScreenLockTag tag);
    }
}