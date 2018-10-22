using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API.ViewModels
{
    /// <summary>
    /// Seleciona todos os valores cadastrados.
    /// </summary>
    public class PlayerViewModel
    {
        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        [Required(ErrorMessage ="preencha o campo nome")]
        public string PlayerName { get; set; }

        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        [MaxLength(30, ErrorMessage ="Máximo de {0} caracteres")]
        public string Sobrenome { get; set; }

        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        [EmailAddress(ErrorMessage = "Prenecha um email válido")]
        [DisplayName("E-Mail")]
        public string EMail { get; set; }


        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        [ScaffoldColumn(false)]
        [DisplayName("Data de cadastro")]
        public TimeZoneInfo DataCadastro { get; set; }

        /// <summary>
        /// Seleciona todos os valores cadastrados.
        /// </summary>
        [ScaffoldColumn(false)]
        [DisplayName("Limite")]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal),"0","999999999999")]
        public decimal Limite { get; set; }

    }
}
