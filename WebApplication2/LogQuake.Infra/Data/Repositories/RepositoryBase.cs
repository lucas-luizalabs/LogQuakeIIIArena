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
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        #region Atributos
        protected LogQuakeContext context;
        private bool _disposed = false;
        #endregion

        #region Construtor da classe
        public RepositoryBase(LogQuakeContext context)
        {
            this.context = context;
        }
        #endregion

        /// <summary>
        /// Método Base de inserir registro no Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser inserido</param>
        public void Add(TEntity obj)
        {
            context.Set<TEntity>().Add(obj);
            //context.SaveChanges();
        }

        /// <summary>
        /// Método Base de buscar todos os registros do Banco de Dados
        /// </summary>
        /// <param name="pageRequest">Define a pagina e tamanho da página a ser utilizada na consulta</param>
        /// <returns>
        /// Retornar uma lista de objetos de acordo com a entidade.
        /// </returns>
        public List<TEntity> GetAll(PageRequestBase pageRequest)
        {
            return context.Set<TEntity>().AsNoTracking().ToList();
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
            //context.SaveChanges();
        }

        /// <summary>
        /// Método Base de alterar um registro do Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser alterado do Banco de Dados</param>
        public void Update(TEntity obj)
        {
            context.Entry(obj).State = EntityState.Modified;
            //context.SaveChanges();
        }

        /// <summary>
        /// Método Base para efetivar/commitar as alterações no Banco de Dados
        /// </summary>
        public void SaveChanges()
        {
            context.SaveChanges();
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
