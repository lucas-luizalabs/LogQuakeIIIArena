using LogQuake.Domain.Context;
using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.EntityConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.Contexto
{
    public class SQLiteLogQuakeContext: LogQuakeContext //DbContext
    {
        //Add-Migration InitialCreate -Context SQLiteLogQuakeContext -OutputDir Migrations\SQLiteMigrations
        //Update-Database -verbose -Context SQLiteLogQuakeContext

        private const string stringConnection = "Data Source=Quake3Arena.db";

        public SQLiteLogQuakeContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite(stringConnection);
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
