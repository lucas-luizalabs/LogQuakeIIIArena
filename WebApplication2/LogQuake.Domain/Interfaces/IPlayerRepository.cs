using LogQuake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IPlayerRepository : IRepositoryBase<Player>
    {
        List<Player> BuscarPorNome(string nome);
    }
}
