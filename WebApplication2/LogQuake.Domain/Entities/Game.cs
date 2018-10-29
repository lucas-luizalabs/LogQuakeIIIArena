using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogQuake.Domain.Entities
{
    public partial class Game
    {
        [JsonProperty("total_kills")]
        public long TotalKills { get; private set; }

        [JsonProperty("players")]
        public string[] Players { get; private set; }

        [JsonProperty("kills")]
        public Dictionary<string, int> Kills = new Dictionary<string, int>();

        public void RegistraPlayers(List<string> players)
        {
            players.Remove("<world>");
            players.Remove(null);

            Players = players.ToArray();
        }

        public void RegistraMorte(string Assassino, string Assassinado)
        {
            TotalKills++;
            //Assasino deve ganhar +1 kill
            if (!string.IsNullOrEmpty(Assassino) && Assassino != "<world>")
            {
                if (Kills.ContainsKey(Assassino))
                {
                    if (Kills[Assassino] + 1 == 0)
                        Kills.Remove(Assassino);
                    else
                        Kills[Assassino] += 1;
                }
                else
                {
                    Kills.Add(Assassino, 1);
                }
            }
            if (!string.IsNullOrEmpty(Assassinado))
            {
                if (!string.IsNullOrEmpty(Assassinado) && Kills.ContainsKey(Assassinado))
                {
                    if (Kills[Assassinado] - 1 == 0)
                        Kills.Remove(Assassinado);
                    else
                        Kills[Assassinado] -= 1;
                }
                else
                {
                    Kills.Add(Assassinado, -1);
                }
            }

        }

    }
}
