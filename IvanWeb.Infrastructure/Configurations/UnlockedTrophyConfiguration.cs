using IvanWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanWeb.Infrastructure.Data.Configurations;

public class UnlockedTrophyConfiguration : IEntityTypeConfiguration<UnlockedTrophy>
{
    public void Configure(EntityTypeBuilder<UnlockedTrophy> builder)
    {
        builder.ToTable("UnlockedTrophies");
        builder.HasKey(ut => ut.Id);

        // Garante que o mesmo jogador não desbloqueie o mesmo troféu duas vezes
        builder.HasIndex(ut => new { ut.PlayerProfileId, ut.TrophyId })
            .IsUnique();
    }
}