using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LogQuake.Domain.Entities
{
    public class Player
    {

        [Key]
        public int Id { get; set; }  // must be public!

        //[JsonProperty("Jogador")]
        public string PlayerName { get; set; }

        //[JsonProperty("Sobrenome")]
        [MaxLength(30)]
        public string Sobrenome { get; set; }
    }
}
