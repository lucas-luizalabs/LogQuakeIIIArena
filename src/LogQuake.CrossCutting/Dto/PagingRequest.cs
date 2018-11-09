
namespace LogQuake.Infra.CrossCuting
{
    public class PagingRequest
    {
        /// <summary>
        /// Página inicial para efetuar a busca de dados
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Quantidade de registros a serem filtrados a partir a página informada
        /// </summary>
        public int PageSize { get; set; } = 5;
    }

}
