using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogQuake.Domain.Entities
{
    public class Player
    {
        [JsonProperty("Jogador")]
        public string PlayerName { get; set; }
    }
}
