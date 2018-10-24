﻿using FluentValidation;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Service.Services
{
    public class ServiceBase<T> : IServiceBase<T> where T : class
    {

        private RepositoryBase<T> repository = new RepositoryBase<T>();

        public void Remove(T obj)
        {
            //if (id == 0)
            //    throw new ArgumentException("The id can't be zero.");

            repository.Remove(obj);
        }

        public IEnumerable<T> GetAll(PageRequestBase pageRequest) => repository.GetAll(pageRequest);
        public T GetById(int id)
        {
            if (id == 0)
                throw new ArgumentException("The id can't be zero.");

            return repository.GetById(id);
        }

        //public IList<T> Get()
        //{

        //    throw new NotImplementedException();
        //}

        public T Add<V>(T obj) where V : AbstractValidator<T>
        {
            Validate(obj, Activator.CreateInstance<V>());

            repository.Add(obj);
            return obj;
        }

        public T Update<V>(T obj) where V : AbstractValidator<T>
        {
            Validate(obj, Activator.CreateInstance<V>());

            repository.Update(obj);
            return obj;
        }
        private void Validate(T obj, AbstractValidator<T> validator)
        {
            if (obj == null)
                throw new Exception("Registros não detectados!");

            validator.ValidateAndThrow(obj);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}