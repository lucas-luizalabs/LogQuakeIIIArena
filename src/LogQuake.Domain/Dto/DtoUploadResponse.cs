using LogQuake.CrossCutting;
using Newtonsoft.Json;

namespace LogQuake.Domain.Dto
{
    public class DtoUploadResponse : DtoResponseBase
    {
        [JsonProperty("FileName")]
        public string FileName { get; set; }

        [JsonProperty("Length")]
        public long Length { get; set; }

        [JsonProperty("RegistrosInseridos")]
        public int RegistrosInseridos { get; set; }
    }

}
