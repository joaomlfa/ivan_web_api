using IvanWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanWeb.Infrastructure.Data.Configurations;

public class PurchaseHistoryConfiguration : IEntityTypeConfiguration<PurchaseHistory>
{
    public void Configure(EntityTypeBuilder<PurchaseHistory> builder)
    {
        builder.ToTable("PurchaseHistories");
        builder.HasKey(ph => ph.Id);

        // Relacionamento com StoreItem (o relacionamento com PlayerProfile já foi feito lá no PlayerProfileConfiguration)
        builder.HasOne(ph => ph.StoreItem)
            .WithMany()
            .HasForeignKey(ph => ph.StoreItemId)
            .OnDelete(DeleteBehavior.Restrict); // Evita deletar um item da loja que já foi comprado
    }
}