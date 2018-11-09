using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repositório de Kill
        /// </summary>
        IKillRepository Kills { get; }

        /// <summary>
        /// Salvar no Banco de Dados todas as mudanças efetuadas em todos os Repositórios 
        /// </summary>
        /// <returns>
        /// Retornar a quantidade de registros afetados.
        /// </returns>
        int Complete();
    }
}
