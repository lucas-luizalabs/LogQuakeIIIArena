using LogQuake.Domain.Entities;
using LogQuake.Infra.CrossCuting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface ILogQuakeService
    {
        List<Game> CarregarLog(string fileName);
        List<Kill> CarregarLogParaDB(string fileName);

        IEnumerable<Kill> GetAll(PageRequestBase pageRequest);
    }
}
