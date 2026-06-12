using Newtonsoft.Json;

namespace Client.Dogs.Models
{
    internal class DogBreedDetailsAttributes
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("life")]
        public LifeSpan Life { get; set; }

        [JsonProperty("male_weight")]
        public Weight MaleWeight { get; set; }

        [JsonProperty("female_weight")]
        public Weight FemaleWeight { get; set; }

        [JsonProperty("hypoallergenic")]
        public bool Hypoallergenic { get; set; }
    }

    internal class LifeSpan
    {
        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }

    internal class Weight
    {
        [JsonProperty("min")]
        public int Min { get; set; }

        [JsonProperty("max")]
        public int Max { get; set; }
    }
}