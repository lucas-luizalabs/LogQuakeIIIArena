using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.Data.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.Repositories
{
    public class KillRepository : RepositoryBase<Kill>, IKillRepository
    {
        //public KillRepository(LogQuakeContext context) : base(context)
        //{
        //}
    }
}
