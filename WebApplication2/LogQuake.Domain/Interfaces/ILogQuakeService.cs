using System.Collections.Generic;
using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;

namespace LogQuake.Service.Services
{
    public interface ILogQuakeService
    {
        List<Game> CarregarLog(string fileName);

        List<Kill> CarregarLogParaDB(List<string> linhas);

        Dictionary<string, _Game> GetAll(PageRequestBase pageRequest);

        Dictionary<string, _Game> GetById(int Id);

        List<string> LerArquivoDeLog(string fileName);
    }
}