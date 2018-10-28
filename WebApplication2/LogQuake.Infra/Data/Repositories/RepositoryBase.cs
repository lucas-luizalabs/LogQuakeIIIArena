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
        protected LogQuakeContext context;
        private bool _disposed = false;

        public RepositoryBase(LogQuakeContext context)
        {
            this.context = context;
        }

        public void Add(TEntity obj)
        {
            context.Set<TEntity>().Add(obj);
            context.SaveChanges();
        }

        public List<TEntity> GetAll(PageRequestBase pageRequest)
        {
            return context.Set<TEntity>().AsNoTracking().ToList();
        }

        public virtual TEntity GetById(int Id)
        {
            return context.Set<TEntity>().Find(Id);
        }

        public void Remove(TEntity obj)
        {
            context.Set<TEntity>().Remove(obj);
            context.SaveChanges();
        }

        public void Update(TEntity obj)
        {
            context.Entry(obj).State = EntityState.Modified;
            context.SaveChanges();
        }

        public int Count()
        {
            return context.Set<TEntity>().Count();
        }

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
