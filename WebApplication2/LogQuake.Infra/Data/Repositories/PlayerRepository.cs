﻿using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.Data.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogQuake.Infra.Data.Repositories
{
    public class PlayerRepository : RepositoryBase<Player>, IPlayerRepository
    {
        public IEnumerable<Player> BuscarPorNome(string nome)
        {
            return context.Set<Player>().Where(p => p.PlayerName == nome).ToList();
        }
    }
}