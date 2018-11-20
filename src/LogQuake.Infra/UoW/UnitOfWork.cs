using LogQuake.Domain.Context;
using LogQuake.Domain.Interfaces;
using LogQuake.Domain.Interfaces.Repositories;
using LogQuake.Infra.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.UoW
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Atributos
        private readonly LogQuakeContext _context;
        #endregion

        #region Contrutor
        /// <summary>
        /// Contrturo da classe
        /// </summary>
        /// <param name="context">Contexto a ser utilizado pelo Unit Of Work</param>
        /// <param name="cache">Objeto de Cache a ser utilizado pelo Unit Of Work</param>
        public UnitOfWork(LogQuakeContext context, IMemoryCache cache, IConfiguration configuration)
        {
            _context = context;
            Kills = new KillRepository(_context, cache, configuration);
        }
        #endregion

        /// <summary>
        /// Repositório de Kill
        /// </summary>
        public IKillRepository Kills { get; private set; }// => throw new NotImplementedException();

        /// <summary>
        /// Salvar no Banco de Dados todas as mudanças efetuadas em todos os Repositórios 
        /// </summary>
        /// <returns>
        /// Retornar a quantidade de registros afetados.
        /// </returns>
        public int Complete()
        {
            return _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Libera da memória todos os recursos utilizados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
