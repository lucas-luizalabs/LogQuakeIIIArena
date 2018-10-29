using LogQuake.Infra.CrossCuting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        void Add(TEntity obj);

        TEntity GetById(int Id);

        List<TEntity> GetAll(PageRequestBase pageRequest);

        void Update(TEntity obj);

        int Count();

        void Remove(TEntity obj);

        void SaveChanges();
        
        void Dispose();
    }
}
