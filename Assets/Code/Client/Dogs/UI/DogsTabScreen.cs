using System;
using System.Collections.Generic;
using System.Threading;
using Client.Dogs.Models;
using Client.Libs.Networking;
using Client.Libs.UI.Contracts;
using Client.Libs.UI.Extensions;
using Client.Libs.UI.Screens;
using Client.Libs.UI.Types;
using Cysharp.Threading.Tasks;

namespace Client.Dogs.UI
{
    [ScreenView(typeof(DogsTabScreenView))]
    internal class DogsTabScreen : UIScreenPresenter
    {
        private const int BREED_LIMIT = 10;

        private readonly DogsTabScreenView _screenView;
        private readonly IDogBreedsRepository _dogBreedsApi;
        private readonly DogBreedDetailsPopupState _popupState;

        private CancellationTokenSource _cts;
        private IQueuedRequestHandle<IReadOnlyList<DogBreedModel>> _breedsRequestHandle;
        private IQueuedRequestHandle<DogBreedDetailsModel> _dogDetailsRequestHandle;

        public DogsTabScreen(
            DogsTabScreenView screenView,
            IScreenManager screenManager,
            IDogBreedsRepository dogBreedsApi,
            DogBreedDetailsPopupState popupState)
            : base(screenView, screenManager)
        {
            _screenView = screenView;
            _dogBreedsApi = dogBreedsApi;
            _popupState = popupState;
        }

        public override ScreenStyle Style => ScreenStyle.Default;

        private CancellationToken LifetimeToken => _cts?.Token ?? CancellationToken.None;

        protected override UniTask OnOpenAsync()
        {
            _cts = new CancellationTokenSource();
            _screenView.DogBreedSelected += OnDogBreedSelected;
            LoadBreeds(_cts.Token).Forget();
            return UniTask.CompletedTask;
        }

        protected override async UniTask OnCloseAsync()
        {
            _screenView.DogBreedSelected -= OnDogBreedSelected;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _breedsRequestHandle?.Cancel();
            _dogDetailsRequestHandle?.Cancel();

            await ScreenManager.Screen<DogBreedDetailsPopupScreen>().Close();
        }

        private async UniTaskVoid LoadBreeds(CancellationToken ct)
        {
            _screenView.SetDogsStatus("Loading breeds...");
            var request = _dogBreedsApi.GetBreeds(BREED_LIMIT);
            _breedsRequestHandle = request;

            try
            {
                var breeds = await request.Task.AttachExternalCancellation(ct);
                _screenView.SetDogsStatus(string.Empty);
                _screenView.SetDogBreeds(breeds);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _screenView.SetDogsStatus(exception.Message);
            }
            finally
            {
                if (ReferenceEquals(_breedsRequestHandle, request))
                    _breedsRequestHandle = null;
            }
        }

        private void OnDogBreedSelected(DogBreedModel breedModel) =>
            SelectDogBreedAsync(breedModel, LifetimeToken).Forget();

        private async UniTaskVoid SelectDogBreedAsync(DogBreedModel breedModel, CancellationToken ct)
        {
            if (breedModel == null)
                return;

            _dogDetailsRequestHandle?.Cancel();
            await ScreenManager.Screen<DogBreedDetailsPopupScreen>().Close();
            ct.ThrowIfCancellationRequested();

            var request = _dogBreedsApi.GetBreedDetails(breedModel);
            _dogDetailsRequestHandle = request;
            _screenView.SetDogsStatus($"Loading {breedModel.Name}...");

            try
            {
                var details = await request.Task.AttachExternalCancellation(ct);
                _screenView.SetDogsStatus(string.Empty);
                _popupState.Set(details);
                await ScreenManager.Screen<DogBreedDetailsPopupScreen>().Open();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _screenView.SetDogsStatus(exception.Message);
            }
            finally
            {
                if (ReferenceEquals(_dogDetailsRequestHandle, request))
                    _dogDetailsRequestHandle = null;
            }
        }
    }
}