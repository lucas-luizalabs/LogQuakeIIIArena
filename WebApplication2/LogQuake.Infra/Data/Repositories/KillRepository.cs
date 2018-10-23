using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogQuake.Infra.Data.Repositories
{
    public class KillRepository : RepositoryBase<Kill>, IKillRepository
    {
        public IEnumerable<Kill> GetAll(PageRequestBase pageRequest)
        {
            return context.Set<Kill>().AsNoTracking().OrderBy(x => x.IdGame)
                .Skip(pageRequest.PageNumber - 1)
                .Take(pageRequest.PageSize).ToList();
        }

    }
}
