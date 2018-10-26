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
        public KillRepository(LogQuakeContext context) : base(context)
        {
        }

        public new List<Kill> GetAll(PageRequestBase pageRequest)
        {
            if (pageRequest == null)
            {
                throw new ArgumentNullException("KillRepository");
            }

            List<Kill> result = new List<Kill>();

            //Buscar jogos agrupados po IdGame
            var resultGroupByIdGame = context.Set<Kill>().OrderBy(i => i.IdGame).Select(p => new { p.IdGame }).GroupBy(i => i.IdGame)
                .Skip(pageRequest.PageNumber - 1)
                .Take(pageRequest.PageSize)
                .ToList();

            if (resultGroupByIdGame.Count == 0)
            {
                return result;
            }


            //filtrar somente o campo IdGame
            List<int> temp = new List<int>();
            foreach (var item in resultGroupByIdGame)
            {
                temp.Add(item.Key);
            }
            if (temp.Count == 0)
            {
                return result;
            }


            result = context.Set<Kill>().Where(x => temp.Contains(x.IdGame)).ToList();

            return result;

        }

    }
}
