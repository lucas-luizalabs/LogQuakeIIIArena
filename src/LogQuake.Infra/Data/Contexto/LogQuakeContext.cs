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
        //private const string SqlServerConnection = "Server=(localdb)\\mssqllocaldb;Database=Quake3Arena;Trusted_Connection=True;";
        private const string SqLiteConnection = "Data Source=Quake3Arena.db";

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
                //optionsBuilder.UseSqlServer(SqlServerConnection);
                optionsBuilder.UseSqlite(SqLiteConnection);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //ou utiliza assim ou o jeito de baixo
            //modelBuilder.Entity<Kill>(new KillConfiguration().Configure);

            modelBuilder.ApplyConfiguration(new KillConfiguration());

            modelBuilder.Entity<Kill>()
                .HasIndex(u => u.IdGame);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
