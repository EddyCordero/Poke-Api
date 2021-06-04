using Newtonsoft.Json;

namespace PokeAPI.Domain.Models
{
    public class Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
