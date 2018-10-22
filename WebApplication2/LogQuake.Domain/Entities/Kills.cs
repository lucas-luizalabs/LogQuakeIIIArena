using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogQuake.Domain.Entities
{
    public partial class Kills
    {
        public Dictionary<string, int> values = new Dictionary<string, int>();
        //values.Add("Dono da bola", 5);
        //values.Add("Isgalamido", 18);
        //values.Add("Zeh", 20);

        //[JsonProperty("Dono da bola")]
        //public long DonoDaBola { get; set; }

        //[JsonProperty("Isgalamido")]
        //public long Isgalamido { get; set; }

        //[JsonProperty("Zeh")]
        //public long Zeh { get; set; }
    }
}
