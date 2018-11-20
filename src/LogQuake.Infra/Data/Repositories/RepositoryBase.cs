using LogQuake.CrossCutting.Cache;
using LogQuake.Domain.Context;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogQuake.Infra.Data.Repositories
{
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        #region Atributos
        protected LogQuakeContext context;
        private bool _disposed = false;
        protected readonly IMemoryCache cache;
        private IConfiguration configuration { get; set; }
        #endregion

        #region Construtor da classe
        public RepositoryBase(LogQuakeContext context, IMemoryCache cache, IConfiguration configuration)
        {
            this.context = context;
            this.cache = cache;
            this.configuration = configuration;
        }
        #endregion

        /// <summary>
        /// Método Base de inserir registro no Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser inserido</param>
        public void Add(TEntity obj)
        {
            context.Set<TEntity>().Add(obj);
        }

        /// <summary>
        /// Método Base de buscar todos os registros do Banco de Dados
        /// </summary>
        /// <param name="pageRequest">Define a pagina e tamanho da página a ser utilizada na consulta</param>
        /// <returns>
        /// Retornar uma lista de objetos de acordo com a entidade.
        /// </returns>
        public List<TEntity> GetAll(PagingRequest pageRequest)
        {
            return context.Set<TEntity>().AsNoTracking().ToList();
        }

        /// <summary>
        /// Buscar genérica de acordo com o predicate
        /// </summary>
        /// <param name="predicate">filtro para verificar se os dados existem</param>
        /// <returns>
        /// Retornar uma lista de registro de acordo com a entidade selecionada.
        /// </returns>
        public List<TEntity> FindBy(Func<TEntity, bool> predicate)
        {
            List<TEntity> query = context.Set<TEntity>().Where(predicate).ToList();
            return query;
        }


        /// <summary>
        /// Buscar primeiramente no Cache de Repositório, caso não encontre nada deve-se buscar no Banco de Dados
        /// </summary>
        /// <param name="predicate">filtro para verificar se os dados existem</param>
        /// <param name="key">Chave para verificar se retorno já existe no Cache</param>
        /// <returns>
        /// Retornar uma lista de registro de acordo com a entidade selecionada.
        /// </returns>
        public List<TEntity> FindByCached(Func<TEntity, bool> predicate, string key)
        {
            List<TEntity> retorno;

            if (!cache.TryGetValue(key, out retorno))
            {
                lock (Cache.lockCache) // ensure concurrent request won't access DB at the same time
                {
                    if (!cache.TryGetValue(key, out retorno)) // double-check
                    {
                        retorno = context.Set<TEntity>().Where(predicate).ToList();
                        var minutes = configuration.GetValue<int>("Cache:RepositoryBase:SlidingExpiration");
                        var size = configuration.GetValue<int>("Cache:RepositoryBase:Size");
                        cache.Set(key, retorno,
                            new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(minutes), Size = size });
                    }
                }
            }

            return retorno;

        }


        /// <summary>
        /// Método Base de buscar um registro por Id do Banco de Dados
        /// </summary>
        /// <param name="Id">Identificador do registro no Banco de Dados</param>
        /// <returns>
        /// Retornar uma objeto de acordo com a entidade.
        /// </returns>
        public virtual TEntity GetById(int Id)
        {
            return context.Set<TEntity>().Find(Id);
        }

        /// <summary>
        /// Método Base de Remover/Excluir um registro do Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser removido do Banco de Dados</param>
        public void Remove(TEntity obj)
        {
            context.Set<TEntity>().Remove(obj);
        }

        /// <summary>
        /// Método Base de alterar um registro do Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser alterado do Banco de Dados</param>
        public void Update(TEntity obj)
        {
            context.Entry(obj).State = EntityState.Modified;
        }

        /// <summary>
        /// Método Base de retornar a quantidade de registros de uma tabela existentes no Banco de Dados
        /// </summary>
        public int Count()
        {
            return context.Set<TEntity>().Count();
        }

        /// <summary>
        /// Método Base para liberar a memória usada sem ter que esperar o Garbage Collector
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
