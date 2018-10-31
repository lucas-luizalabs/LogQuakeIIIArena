using System.Collections.Generic;
using FluentValidation;
using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;

namespace LogQuake.Service.Services
{
    public interface ILogQuakeService
    {
        /// <summary>
        /// Adicionar no Banco de Dados uma lista Kills.
        /// </summary>
        /// <param name="Kills">lista de kills</param>
        /// <returns>
        /// Retorna a quantidade de registros inseridos no Banco de Dados.
        /// </returns>
        int AddKillListInDB(List<Kill> Kills);

        /// <summary>
        /// Método responsável por converter as linhas do arquivo de log contidas em uma lista de strings, em uma lista de Kills.
        /// </summary>
        /// <param name="linhas">lista de string, contendo as linhas do arquivo de log</param>
        /// <returns>
        /// Retorna uma lista de Kills.
        /// </returns>
        List<Kill> ConvertLogFileInListKill(List<string> linhas);

        /// <summary>
        /// Busca no Banco de Dados os dados de todos os jogos, respeitando a paginação informada.
        /// </summary>
        /// <param name="pageRequest">parâmetros de paginação para buscar no Banco de Dados</param>
        Dictionary<string, Game> GetAll(PageRequestBase pageRequest);

        /// <summary>
        /// Busca no Banco de Dados os dados de um determinado Jogo.
        /// </summary>
        /// <param name="Id">Identificador do Jogo</param>
        Dictionary<string, Game> GetById(int Id);

        /// <summary>
        /// Método responsável por ler o arquivo de log do jogo Quake 3 Arena e criar uma lista de string, contendo as linhas do log.
        /// </summary>
        /// <param name="filename">arquivo a ser lido</param>
        /// <returns>
        /// Retorna uma lista de string, contendo as linhas do arquivo de log lido.
        /// </returns>
        List<string> ReadLogFile(string fileName);
    }
}