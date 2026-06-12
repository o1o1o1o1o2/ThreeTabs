using Newtonsoft.Json;

namespace Client.Dogs.Models
{
    internal class DogBreedAttributes
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}