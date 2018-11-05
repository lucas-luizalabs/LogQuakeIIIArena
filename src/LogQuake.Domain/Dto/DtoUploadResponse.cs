using LogQuake.CrossCutting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
