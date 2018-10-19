using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogQuake.Domain.Entities
{
    public partial class _Game
    {
        [JsonProperty("total_kills")]
        public long TotalKills { get; set; }

        [JsonProperty("players")]
        public string[] Players { get; set; }

        [JsonProperty("kills")]
        public Kills Kills { get; set; }
    }
}
