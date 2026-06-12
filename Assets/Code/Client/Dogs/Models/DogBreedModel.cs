using Newtonsoft.Json;

namespace Client.Dogs.Models
{
    internal class DogBreedModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public DogBreedAttributes Attributes { get; set; }

        [JsonIgnore]
        public string Name => Attributes?.Name;
        [JsonIgnore]
        public string Description => Attributes?.Description;
    }
}