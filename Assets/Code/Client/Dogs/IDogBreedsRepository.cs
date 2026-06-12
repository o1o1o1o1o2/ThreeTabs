using System.Collections.Generic;
using Client.Dogs.Models;
using Client.Libs.Networking;

namespace Client.Dogs
{
    internal interface IDogBreedsRepository
    {
        IQueuedRequestHandle<IReadOnlyList<DogBreedModel>> GetBreeds(int limit);
        IQueuedRequestHandle<DogBreedDetailsModel> GetBreedDetails(DogBreedModel breedModel);
    }
}