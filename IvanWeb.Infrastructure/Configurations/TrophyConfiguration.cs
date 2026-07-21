using IvanWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanWeb.Infrastructure.Data.Configurations;

public class TrophyConfiguration : IEntityTypeConfiguration<Trophy>
{
    public void Configure(EntityTypeBuilder<Trophy> builder)
    {
        // Nome da tabela (opcional, o EF já pluraliza, mas é bom ser explícito)
        builder.ToTable("Trophies");

        // Chave Primária
        builder.HasKey(t => t.Id);

        // O Slug precisa ser único e ter tamanho máximo para otimizar o banco
        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        // Armazenando o Enum como string no banco fica muito mais fácil de ler 
        // caso você abra o PGAdmin depois (ex: vai salvar "PrimeiraVez" em vez de "0")
        builder.Property(t => t.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Rarity)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}