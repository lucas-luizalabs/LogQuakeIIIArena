using Newtonsoft.Json;
using System.Collections.Generic;

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

        /// <summary>
        /// Adiciona na lista Players os jogadores informados, removendo Nulos e world
        /// </summary>
        /// <param name="players">lista de jogadores</param>
        public void RegisterPlayers(List<string> players)
        {
            if (players != null)
            {
                players.Remove("<world>");
                players.Remove(null);

                Players = players.ToArray();
            }
        }

        /// <summary>
        /// Registra morte na Lista Kills, para cada Player e incrementa o contador total de mortes "TotalKills"
        /// </summary>
        /// <param name="killer">nome do assassino</param>
        /// <param name="killed">nome do assassinado</param>
        public void RegisterDeath(string killer, string killed)
        {
            TotalKills++;
            //Assasino deve ganhar +1 kill
            if (!string.IsNullOrEmpty(killer) && killer != "<world>")
            {
                if (Kills.ContainsKey(killer)) //Assassino
                {
                    if (Kills[killer] + 1 == 0)
                        Kills.Remove(killer);
                    else
                        Kills[killer] += 1;
                }
                else
                {
                    Kills.Add(killer, 1);
                }
            }
            if (!string.IsNullOrEmpty(killed)) //Assassinado
            {
                if (!string.IsNullOrEmpty(killed) && Kills.ContainsKey(killed))
                {
                    if (Kills[killed] - 1 == 0)
                        Kills.Remove(killed);
                    else
                        Kills[killed] -= 1;
                }
                else
                {
                    Kills.Add(killed, -1);
                }
            }
        }
    }
}
