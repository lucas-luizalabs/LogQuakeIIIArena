using LogQuake.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogQuake.Infra.Data.EntityConfig
{
    public class KillConfiguration : IEntityTypeConfiguration<Kill>
    {
        public void Configure(EntityTypeBuilder<Kill> builder)
        {
            //Nome da tabela
            builder.ToTable("Kill");

            //Chave primária
            builder.HasKey(c => c.Id);

            //Definindo as propriedades de cada coluna da tabela
            builder.Property(c => c.PlayerKiller)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.IdGame)
                .IsRequired();

            builder.Property(c => c.PlayerKilled)
                .HasMaxLength(30)
                .IsRequired();

        }
    }
}
