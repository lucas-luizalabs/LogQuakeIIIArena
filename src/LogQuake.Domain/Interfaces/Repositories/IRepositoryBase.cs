using LogQuake.Infra.CrossCuting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        void Add(TEntity obj);

        TEntity GetById(int Id);

        List<TEntity> GetAll(PagingRequest pageRequest);

        List<TEntity> FindBy(Func<TEntity, bool> predicate);

        List<TEntity> FindByCached(Func<TEntity, bool> predicate, string key);

        void Update(TEntity obj);

        int Count();

        void Remove(TEntity obj);

        void Dispose();
    }
}
