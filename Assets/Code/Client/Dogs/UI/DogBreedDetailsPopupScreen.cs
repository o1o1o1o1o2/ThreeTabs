using Client.Libs.UI.Contracts;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;

namespace Client.Dogs.UI
{
    [ScreenView(typeof(DogBreedDetailsPopupScreenView))]
    internal sealed class DogBreedDetailsPopupScreen : UIScreenPresenter
    {
        private readonly DogBreedDetailsPopupScreenView _screenView;
        private readonly DogBreedDetailsPopupState _state;

        public DogBreedDetailsPopupScreen(
            DogBreedDetailsPopupScreenView screenView,
            IScreenManager screenManager,
            DogBreedDetailsPopupState state)
            : base(screenView, screenManager)
        {
            _screenView = screenView;
            _state = state;
        }

        public override ScreenStyle Style => ScreenStyle.AlwaysOnTop;

        protected override UniTask OnOpenAsync()
        {
            _screenView.CloseClicked += Close;
            _screenView.SetDetails(_state.Details);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnCloseAsync()
        {
            _screenView.CloseClicked -= Close;
            return UniTask.CompletedTask;
        }
    }
}
