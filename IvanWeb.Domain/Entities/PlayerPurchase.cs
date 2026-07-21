namespace IvanWeb.Domain.Entities;

public class PlayerPurchase
{
    public Guid Id { get; set; }
    public Guid PlayerProfileId { get; set; }
    public Guid StoreItemId { get; set; }
    public DateTime PurchasedAt { get; set; }

    // Propriedades de Navegação (Entity Framework)
    public PlayerProfile? Player { get; set; }
    public StoreItem? StoreItem { get; set; }
}