using System.Collections.Generic;
using System.IO;
using FluentValidation;
using LogQuake.Domain.Dto;
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
        DtoGameResponse GetAll(PagingRequest pageRequest);

        /// <summary>
        /// Busca no Banco de Dados os dados de um determinado Jogo.
        /// </summary>
        /// <param name="Id">Identificador do Jogo</param>
        //Dictionary<string, Game> GetById(int Id);
        DtoGameResponse GetById(int Id);

        /// <summary>
        /// Método responsável por ler o arquivo de log do jogo Quake 3 Arena e criar uma lista de string, contendo as linhas do log.
        /// </summary>
        /// <param name="filename">arquivo a ser lido</param>
        /// <returns>
        /// Retorna uma lista de string, contendo as linhas do arquivo de log lido.
        /// </returns>
        List<string> ReadLogFile(string fileName);

        /// <summary>
        /// Método responsável receber o Upload do arquivo de log.
        /// </summary>
        /// <param name="folder">local de destino do arquivo de log</param>
        /// <param name="filename">arquivo a ser lido</param>
        /// <param name="stream">arquivo propriamente dito no formato Stream</param>
        /// <returns>
        /// Retorna um objeto contendo nome do arquivo, quantidade de registro inseridos, se ouve sucesso Sim ou Não.
        /// </returns>
        DtoUploadResponse UploadFile(string folder, string fileName, Stream stream);
    }
}