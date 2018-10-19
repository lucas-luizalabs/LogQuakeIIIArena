using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogQuake.Domain.Entities
{
    public partial class Games
    {
        [JsonProperty("games")]
        public _Game[] GamesGames { get; set; }
    }

    public partial class Games
    {
        public static Games FromJson(string json) => JsonConvert.DeserializeObject<Games>(json, LogQuake.Domain.Entities.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Games self) => JsonConvert.SerializeObject(self, LogQuake.Domain.Entities.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
