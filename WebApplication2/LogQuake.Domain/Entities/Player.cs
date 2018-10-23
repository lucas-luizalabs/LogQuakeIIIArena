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
        public int Id { get; set; }  

        public string PlayerName { get; set; }
    }
}
