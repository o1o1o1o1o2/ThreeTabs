using System;
using System.Collections.Generic;
using Client.Dogs.Models;
using Client.Infrastructure;
using Client.Libs.Networking;
using Newtonsoft.Json.Linq;

namespace Client.Dogs
{
    internal sealed class DogBreedsRepository : IDogBreedsRepository
    {
        private const string BASE_URL = "https://dogapi.dog/api/v2";
        private readonly QueuedHttpClient _httpClient;

        public DogBreedsRepository(QueuedHttpClient httpClient) =>
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public IQueuedRequestHandle<IReadOnlyList<DogBreedModel>> GetBreeds(int limit)
        {
            var httpRequest = _httpClient.GetText($"{BASE_URL}/breeds?limit={limit}", "dog breeds");
            return new MappedQueuedRequestHandle<string, IReadOnlyList<DogBreedModel>>(httpRequest, ParseBreeds);
        }

        public IQueuedRequestHandle<DogBreedDetailsModel> GetBreedDetails(DogBreedModel breedModel)
        {
            var httpRequest = _httpClient.GetText($"{BASE_URL}/breeds/{breedModel.Id}", $"dog breed {breedModel.Id}");
            return new MappedQueuedRequestHandle<string, DogBreedDetailsModel>(httpRequest, ParseBreedDetails);
        }

        private static IReadOnlyList<DogBreedModel> ParseBreeds(string json)
        {
            var dataToken = JObject.Parse(json)["data"];
            return dataToken == null
                ? Array.Empty<DogBreedModel>()
                : dataToken.ToObject<List<DogBreedModel>>();
        }

        private static DogBreedDetailsModel ParseBreedDetails(string json)
        {
            var dataToken = JObject.Parse(json)["data"];
            return dataToken == null
                ? throw new InvalidOperationException("Breed details response does not contain 'data' field.")
                : dataToken.ToObject<DogBreedDetailsModel>();
        }
    }
}