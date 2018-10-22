using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Entities
{
    public class Game
    {

        public int Id { get; set; }

        public long TotalKills { get; set; }

        [JsonProperty("players")]
        public virtual List<Player> Player { get; set; }

        [JsonProperty("kills")]
        public virtual Kills Kills { get; set; }
    }
}
