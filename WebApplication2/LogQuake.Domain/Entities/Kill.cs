using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LogQuake.Domain.Entities
{
    public class Kill
    {
        [Key]
        public int Id { get; set; }

        public int IdPlayer { get; set; }

        public int IdGame { get; set; }

        public int CountKill { get; set; }
    }
}
