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
            var xxx = context.Set<Kill>().OrderBy(i => i.IdGame).Select(p => new { p.IdGame }).GroupBy(i => i.IdGame)
                .Skip(pageRequest.PageNumber - 1)
                .Take(pageRequest.PageSize)
                .ToList();

            List<int> ccc = new List<int>();
            foreach (var item in xxx)
            {
                ccc.Add(item.Key);
            }


            return context.Set<Kill>().Where(x => ccc.Contains(x.IdGame)).ToList();

            return context.Set<Kill>().AsNoTracking()
                .GroupBy(i => i.IdGame)
                .Select(g => g.First())
                .OrderBy(x => x.IdGame)
                .Skip(pageRequest.PageNumber - 1)
                .Take(pageRequest.PageSize)
                .ToList();
        }

    }
}
