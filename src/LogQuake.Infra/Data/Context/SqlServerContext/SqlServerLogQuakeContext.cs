using LogQuake.Domain.Context;
using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.EntityConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.SqlServerContext
{
    public class SqlServerLogQuakeContext: LogQuakeContext //DbContext
    {
        //Add-Migration InitialCreate -Context SqlServerLogQuakeContext -OutputDir Migrations\SqlServerMigrations
        //Update-Database -verbose -Context SqlServerLogQuakeContext

        private const string stringConnection = "Server=(localdb)\\mssqllocaldb;Database=Quake3Arena;Trusted_Connection=True;";

        public SqlServerLogQuakeContext(DbContextOptions options)
          : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(stringConnection);
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
    }
}
