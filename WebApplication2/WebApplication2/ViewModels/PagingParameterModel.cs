using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogQuake.API.ViewModels
{
    /// <summary>
    /// Classe utilizada na camada de Apresentation utilizada para paginação
    /// </summary>
    public class PagingParameterModel
    {
        /// <summary>
        /// Construtor da classe de paginação
        /// </summary>
        public PagingParameterModel() { }

        const int maxPageSize = 20;

        /// <summary>
        /// Definir a página
        /// </summary>
        public int pageNumber { get; set; } = 1;

        /// <summary>
        /// Definir o tamanho da página
        /// </summary>
        public int _pageSize { get; set; } = 10;

        /// <summary>
        /// Definir o tamanho da página
        /// </summary>
        public int pageSize
        {

            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
