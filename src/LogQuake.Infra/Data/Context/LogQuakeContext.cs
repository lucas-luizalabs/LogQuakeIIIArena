using LogQuake.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Domain.Context
{
    public abstract class LogQuakeContext: DbContext
    {
        public LogQuakeContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Acesso aos dados da tabela Kill
        /// </summary>
        public virtual DbSet<Kill> Kills { get; set; }

        /// <summary>
        /// Salva no Banco de Dados todas as mudanças efetuadas no Contexto
        /// </summary>
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
