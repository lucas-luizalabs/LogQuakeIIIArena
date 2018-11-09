using LogQuake.CrossCutting;
using LogQuake.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LogQuake.Domain.Dto
{
    public class DtoGameResponse: DtoResponseBase
    {
        [JsonProperty("game")]
        public Dictionary<string, Game> Game;
    }
}
