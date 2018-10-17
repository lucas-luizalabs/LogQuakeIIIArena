using LogQuake.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.Contexto
{
    public class LogQuakeContext: DbContext
    {
        private const string V = "LogQuake";

        //public LogQuakeContext()
        //    : base(V)
        //{ }

        public LogQuakeContext(DbContextOptions<LogQuakeContext> options)
          : base(options)
        { }

        public DbSet<Player> Players { get; set; }

        
    }
}
