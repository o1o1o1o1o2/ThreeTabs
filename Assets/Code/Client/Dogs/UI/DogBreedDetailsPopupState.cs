using Client.Dogs.Models;

namespace Client.Dogs.UI
{
    internal sealed class DogBreedDetailsPopupState
    {
        public DogBreedDetailsModel Details { get; private set; }

        public void Set(DogBreedDetailsModel details) =>
            Details = details;
    }
}
