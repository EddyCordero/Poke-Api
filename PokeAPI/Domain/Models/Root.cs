using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeAPI.Domain.Models
{
    public class Root
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }

        [JsonProperty("results")]
        public List<Item> results { get; set; }
    }
}
