using System.Collections.Generic;
using FluentValidation;
using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;

namespace LogQuake.Service.Services
{
    public interface ILogQuakeService
    {
        int AdicionarEmBDListaDeKill(List<Kill> Kills);
            
        List<Kill> ConverterArquivoEmListaDeKill(List<string> linhas);

        Dictionary<string, Game> GetAll(PageRequestBase pageRequest);

        Dictionary<string, Game> GetById(int Id);

        List<string> LerArquivoDeLog(string fileName);
    }
}