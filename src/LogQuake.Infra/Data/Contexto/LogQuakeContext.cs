using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.EntityConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.Contexto
{
    public class LogQuakeContext: DbContext
    {
        private const string stringConnection = "Server=(localdb)\\mssqllocaldb;Database=Quake3Arena;Trusted_Connection=True;";

        public LogQuakeContext()
        {
        }

        public LogQuakeContext(DbContextOptions<LogQuakeContext> options)
          : base(options)
        {
        }

        /// <summary>
        /// Acesso aos dados da tabela Kill
        /// </summary>
        public DbSet<Kill> Kills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(stringConnection);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ou utiliza assim ou o jeito de baixo
            //modelBuilder.Entity<Player>(new PlayerConfiguration().Configure);

            modelBuilder.ApplyConfiguration(new KillConfiguration());
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
