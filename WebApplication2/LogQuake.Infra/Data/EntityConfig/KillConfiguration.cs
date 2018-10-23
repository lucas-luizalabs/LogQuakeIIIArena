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
            builder.ToTable("Kill");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.PlayerKiller)
                .IsRequired();

            builder.Property(c => c.IdGame)
                .IsRequired();

            builder.Property(c => c.PlayerKilled)
                .IsRequired();

        }
    }
}
