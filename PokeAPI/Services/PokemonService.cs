using Newtonsoft.Json;
using PokeAPI.Domain.Helpers;
using PokeAPI.Domain.Models;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
//using System.Text.Json;
using System.Threading.Tasks;

namespace PokeAPI.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly string baseUrl = AppSettings.BaseURL;
        private readonly int resultLimit = AppSettings.ResultLimit;
        static HttpClient client = new HttpClient();


        public async Task<List<Item>> GetAllAsync()
        {
            var streamTask = await client.GetAsync($"{baseUrl}/?limit={resultLimit}");
            //var riskEntries =  JsonSerializer.Deserialize<Root>(streamTask);

            using var sr = new System.IO.StreamReader(await streamTask.Content.ReadAsStreamAsync());
            using JsonReader reader = new JsonTextReader(sr);
            var serializer = JsonSerializer.Create();
            var root = serializer.Deserialize<Root>(reader);

            return root.results;
        }

        public async Task<Pokemon> GetByNameAsync(string name)
        {
            var streamTask = await client.GetAsync($"{baseUrl}/{name}");

            var sr = new System.IO.StreamReader(await streamTask.Content.ReadAsStreamAsync());
            using JsonReader reader = new JsonTextReader(sr);
            var serializer = JsonSerializer.Create();
            var pokemon = serializer.Deserialize<Pokemon>(reader);

            return pokemon;
        }

        private T DeserializeStream<T>(Stream stream)
        {
            using var sr = new System.IO.StreamReader(stream);
            using JsonReader reader = new JsonTextReader(sr);
            var serializer = JsonSerializer.Create();
            return serializer.Deserialize<T>(reader);
        }
    }

    public interface IPokemonService
    {
        Task<List<Item>> GetAllAsync();
        Task<Pokemon> GetByNameAsync(string name);
    }
}
