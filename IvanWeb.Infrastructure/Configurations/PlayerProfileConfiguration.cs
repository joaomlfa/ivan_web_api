using IvanWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanWeb.Infrastructure.Data.Configurations;

public class PlayerProfileConfiguration : IEntityTypeConfiguration<PlayerProfile>
{
    public void Configure(EntityTypeBuilder<PlayerProfile> builder)
    {
        // Define o nome da tabela
        builder.ToTable("PlayerProfiles");

        // Chave Primária herdada do EntityBase
        builder.HasKey(p => p.Id);

        // Propriedades do perfil
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.TotalScore)
            .IsRequired()
            .HasDefaultValue(0);

        // Relacionamentos (1:N)

        // Um Jogador tem muitos Troféus Desbloqueados
        builder.HasMany(p => p.UnlockedTrophies)
            .WithOne(ut => ut.PlayerProfile)
            .HasForeignKey(ut => ut.PlayerProfileId)
            .OnDelete(DeleteBehavior.Cascade); // Se deletar o jogador, deleta o histórico de troféus dele

        // Um Jogador tem muitas Compras na Loja
        builder.HasMany(p => p.Purchases)
            .WithOne(ph => ph.PlayerProfile)
            .HasForeignKey(ph => ph.PlayerProfileId)
            .OnDelete(DeleteBehavior.Cascade); // Se deletar o jogador, deleta o histórico de compras
    }
}