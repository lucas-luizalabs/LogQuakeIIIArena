using LogQuake.Domain.Entities;
using LogQuake.Infra.Data.EntityConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.Contexto
{
    public class LogQuakeContext: DbContext
    {
        //private const string V = "LogQuakeDatabase";
        private const string V = "Server=(localdb)\\mssqllocaldb;Database=EFGetStarted.ConsoleApp.NewDb;Trusted_Connection=True;";
        //private DbContextOptions<LogQuakeContext> options = new DbContextOptions<LogQuakeContext>();

        //public LogQuakeContext()
        //    : base(V)
        //{ }

        public LogQuakeContext()
        {
        }

        public LogQuakeContext(DbContextOptions<LogQuakeContext> options)
          : base(options)
        { }

        public DbSet<Kill> Kills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(V);
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
            //foreach (var item in ChangeTracker.Entries().Equals)
            //{

            //}
            return base.SaveChanges();
        }
    }
}
