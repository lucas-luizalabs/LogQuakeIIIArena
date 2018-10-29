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
        /// <summary>
        /// Consutrtor da classe
        /// </summary>
        public KillRepository(LogQuakeContext context) : base(context)
        {
        }

        /// <summary>
        /// Remover/Excluir todos os registro da tabela Kill
        /// </summary>
        public void RemoveAll()
        {
            context.Kills.RemoveRange(context.Kills);
            //context.SaveChanges();
        }

        /// <summary>
        /// Buscar todos os registros da tabela Kill de forma paginada
        /// </summary>
        /// <param name="pageRequest">Define a página e o tamanho da página a ser utilizada na consulta</param>
        /// <returns>
        /// Retornar uma lista de registro da tabela Kill.
        /// </returns>
        public new List<Kill> GetAll(PageRequestBase pageRequest)
        {
            if (pageRequest == null)
            {
                throw new ArgumentNullException("KillRepository");
            }

            List<Kill> result = new List<Kill>();

            //Buscar jogos agrupados po IdGame
            var resultGroupByIdGame = context.Set<Kill>().OrderBy(i => i.IdGame).Select(p => new { p.IdGame }).GroupBy(i => i.IdGame)
                .Skip(((pageRequest.PageNumber - 1) * pageRequest.PageSize))
                .Take(pageRequest.PageSize)
                .ToList();

            if (resultGroupByIdGame.Count == 0)
            {
                return result;
            }

            //Criar uma lista com somente o campo IdGame
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

        /// <summary>
        /// Buscar registros da tabela Kill por Id
        /// </summary>
        /// <param name="Id">Identificador da tabela Kill</param>
        /// <returns>
        /// Retornar uma lista de registro da tabela Kill.
        /// </returns>
        public List<Kill> GetByIdList(int Id)
        {
            return context.Set<Kill>().Where(x => x.IdGame == Id).ToList();
        }

    }
}
