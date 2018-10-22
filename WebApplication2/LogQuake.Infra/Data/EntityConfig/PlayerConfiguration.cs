using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using LogQuake.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogQuake.Infra.Data.EntityConfig
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("Player");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.PlayerName)
                .IsRequired()
                .HasMaxLength(60)
                .HasColumnName("PlayerName");

            builder.Property(c => c.Sobrenome)
                .IsRequired()
                .HasMaxLength(60)
                .HasColumnName("Sobrenome");

        }
    }
}
