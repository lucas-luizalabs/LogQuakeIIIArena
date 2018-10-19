using LogQuake.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Infra.Data.EntityConfig
{
    public class KillConfiguration : IEntityTypeConfiguration<Kill>
    {
        public void Configure(EntityTypeBuilder<Kill> builder)
        {
            builder.ToTable("Kills");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.IdPlayer)
                .IsRequired();

            builder.Property(c => c.IdGame)
                .IsRequired();

            //// Message
            //builder.HasOne(c => c.IdPlayer)
            //       .WithOne(c => c)
            //       .HasForeignKey<Player>(c => c.)
            //       .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne<Player>(s => s.IdPlayer)
            //.WithMany()
            //.HasForeignKey(e => e.IdPlayer);

            //builder.OwnsOne(c => c.IdPlayer);
        }
    }
}
