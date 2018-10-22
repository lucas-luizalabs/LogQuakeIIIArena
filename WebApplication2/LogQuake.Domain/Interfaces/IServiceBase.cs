using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces
{
    public interface IServiceBase<T> where T : class
    {
        T Post<V>(T obj) where V : AbstractValidator<T>;

        T Put<V>(T obj) where V : AbstractValidator<T>;

        void Delete(T obj);

        T Get(int id);

        IEnumerable<T> Get();
    }
}
