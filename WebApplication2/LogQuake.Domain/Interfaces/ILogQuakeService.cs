using System.Collections.Generic;
using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;

namespace LogQuake.Service.Services
{
    public interface ILogQuakeService
    {
        List<Game> CarregarLog(string fileName);
        List<Kill> CarregarLogParaDB(string fileName);
        List<_Game> GetAll(PageRequestBase pageRequest);
    }
}