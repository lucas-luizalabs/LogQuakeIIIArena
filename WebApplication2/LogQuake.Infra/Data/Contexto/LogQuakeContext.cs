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
        //private const string V = "LogQuake";

        //public LogQuakeContext()
        //    : base(V)
        //{ }

        public LogQuakeContext(DbContextOptions<LogQuakeContext> options)
          : base(options)
        { }

        public DbSet<Player> Players { get; set; }

        public DbSet<Kill> Kills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Player>(b =>
            //{
            //    b.HasKey(u => u.Id);
            //    b.HasIndex(u => u.PlayerName).HasName("PlayerName").IsUnique();
            //    b.Property(u => u.PlayerName).HasColumnType("varchar");
            //    b.Property(u => u.PlayerName).HasMaxLength(100);
            //    b.ToTable("Players");
            //});

            //modelBuilder.Entity<Kill>()
            //    .HasOne(p => p.IdPlayer);

            //modelBuilder.Entity<Kill>()
            //            .HasOne(c => c.IdPlayer)
            //            .WithOne()
            //            .HasForeignKey(c => c.MessageId);

            modelBuilder.ApplyConfiguration(new PlayerConfiguration());

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
