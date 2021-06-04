using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PokeAPI.Domain.Helpers;
using PokeAPI.Domain.Models;
using PokeAPI.Extensions;
using PokeAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly string cacheKey = "allPokemons";
        private readonly IPokemonService _pokemonService;
        private IMemoryCache _memoryCache;

        public PokemonController(IPokemonService pokemonService, IMemoryCache memoryCache)
        {
            _pokemonService = pokemonService;
            _memoryCache = memoryCache;
        }
     
        [HttpGet]
        public async Task<IActionResult> SearchByNameAsync(string name)
       {
            if (string.IsNullOrWhiteSpace(name)) return NotFound();

            var list = new List<Item>();

            if (_memoryCache != null && ((MemoryCache)_memoryCache).Count > 0)
            {
                list.AddRange(_memoryCache.Get<List<Item>>(cacheKey));
            } 
            else
            {
                var pokemons = await _pokemonService.GetAllAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                  .SetSlidingExpiration(TimeSpan.FromMinutes(AppSettings.CacheExpirationInMinutes));

                if (pokemons.Count > 0)
                {
                    _memoryCache.Set(cacheKey, pokemons, cacheEntryOptions);

                    list.AddRange(pokemons);
                }
            }
            name = name.SanitizedName();

            list = list.Where(x => x.Name.Contains(name)).ToList();

            if (list.Count == 0) return NotFound();

            return Ok(list);
        }


        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return NotFound();

            var pokemon = await _pokemonService.GetByNameAsync(name);

            if (pokemon == null) return NotFound($"El pokemon {name} no fue encontrado");

            return Ok(pokemon);
        }

        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string name)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter objstreamwriter = new StreamWriter(stream);
                
                if (string.IsNullOrWhiteSpace(name))
                {
                    objstreamwriter.Write("El pokemon no fue encontrado");
                } 
                else
                {
                    var pokemon = await _pokemonService.GetByNameAsync(name);

                    if (pokemon != null)
                    {
                        objstreamwriter.Write("Detalle de Pokemon \r\n");
                        objstreamwriter.Write($"Name : {pokemon.name} \r\n");
                        objstreamwriter.Write($"Base Experience : {pokemon.base_experience} \r\n");
                        objstreamwriter.Write($"Height : {pokemon.height} \r\n");
                        objstreamwriter.Write($"Weight : {pokemon.weight} \r\n");

                        objstreamwriter.Write("");
                        objstreamwriter.Write($"Forms \r\n");

                        for (int i = 0; i < pokemon.forms.Count; i++)
                        {
                            objstreamwriter.Write($"Form: {pokemon.forms[i].name} \r\n");
                        }

                        objstreamwriter.Write("");
                        objstreamwriter.Write($"Abilities \r\n");

                        for (int i = 0; i < pokemon.Abilities.Count; i++)
                        {
                            objstreamwriter.Write($"Ability: {pokemon.Abilities[i].ability.Name} \r\n");
                            objstreamwriter.Write($"Is Hidden: {pokemon.Abilities[i].is_hidden} \r\n");
                        }

                        objstreamwriter.Flush();
                        objstreamwriter.Close();
                    }
                }

                return File(stream.ToArray(), "text/plain", $"{name}-file.txt");
            }
        }
    }
}
