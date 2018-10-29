using FluentValidation;
using LogQuake.Infra.CrossCuting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IServiceBase<T> where T : class
    {
        T Add<V>(T obj) where V : AbstractValidator<T>;

        T GetById(int id);

        IEnumerable<T> GetAll(PageRequestBase pageRequest);

        T Update<V>(T obj) where V : AbstractValidator<T>;

        void Remove(T obj);

        void Dispose();
    }
}
