using FluentValidation;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.CrossCuting;
using LogQuake.Infra.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Service.Services
{
    /// <summary>
    /// Classe de Serviço Base para todas as outras que necessitem efetuar um CRUD.
    /// </summary>
    public class ServiceBase<T> : IServiceBase<T> where T : class
    {
        /// <summary>
        /// Criação de repositório recebendo uma classe.
        /// </summary>
        private RepositoryBase<T> repository;

        /// <summary>
        /// Construtor da classe de Serviço Base, recebendo o repositório que irá trabalhar.
        /// </summary>
        public ServiceBase(RepositoryBase<T> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Método de Remover/Excluir um registro da tabela.
        /// </summary>
        public void Remove(T obj)
        {
            repository.Remove(obj);
        }

        /// <summary>
        /// Método para buscar todos os registros do repositório T de forma paginada.
        /// </summary>
        public IEnumerable<T> GetAll(PageRequestBase pageRequest) => repository.GetAll(pageRequest);


        /// <summary>
        /// Método para buscar registros por id do repositório T.
        /// </summary>
        public T GetById(int id)
        {
            if (id == 0)
                throw new ArgumentException("id não pode ser zero.");

            return repository.GetById(id);
        }

        /// <summary>
        /// Método para adicionar um registro ao repositório T.
        /// </summary>
        public T Add<V>(T obj) where V : AbstractValidator<T>
        {
            Validate(obj, Activator.CreateInstance<V>());

            repository.Add(obj);
            return obj;
        }

        /// <summary>
        /// Método para alterar um registro no repositório T.
        /// </summary>
        public T Update<V>(T obj) where V : AbstractValidator<T>
        {
            Validate(obj, Activator.CreateInstance<V>());

            repository.Update(obj);
            return obj;
        }

        /// <summary>
        /// Método para validar um registro do repositório T.
        /// </summary>
        private void Validate(T obj, AbstractValidator<T> validator)
        {
            if (obj == null)
                throw new Exception("Registros não detectados!");

            validator.ValidateAndThrow(obj);
        }

        /// <summary>
        /// Método para descarregar da memória a classe de Serviço Base.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
