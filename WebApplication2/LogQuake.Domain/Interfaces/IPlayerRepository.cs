using LogQuake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        IEnumerable<Player> BuscarPorNome(string nome);
    }
}
