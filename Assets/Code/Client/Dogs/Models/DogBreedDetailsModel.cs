using Newtonsoft.Json;

namespace Client.Dogs.Models
{
    internal class DogBreedDetailsModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public DogBreedDetailsAttributes Attributes { get; set; }

        [JsonIgnore]
        public string Name => Attributes?.Name;
        [JsonIgnore]
        public string Description => Attributes?.Description;
        [JsonIgnore]
        public int LifeMin => Attributes?.Life?.Min ?? 0;
        [JsonIgnore]
        public int LifeMax => Attributes?.Life?.Max ?? 0;
        [JsonIgnore]
        public int MaleWeightMin => Attributes?.MaleWeight?.Min ?? 0;
        [JsonIgnore]
        public int MaleWeightMax => Attributes?.MaleWeight?.Max ?? 0;
        [JsonIgnore]
        public int FemaleWeightMin => Attributes?.FemaleWeight?.Min ?? 0;
        [JsonIgnore]
        public int FemaleWeightMax => Attributes?.FemaleWeight?.Max ?? 0;
        [JsonIgnore]
        public bool Hypoallergenic => Attributes?.Hypoallergenic ?? false;
    }
}