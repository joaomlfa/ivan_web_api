namespace IvanWeb.Domain.Entities;

public class UnlockedTrophy : EntityBase
{
    public Guid PlayerProfileId { get; set; }
    public Guid TrophyId { get; set; }
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

    // Propriedades de Navegação
    public PlayerProfile? PlayerProfile { get; set; }
    public Trophy? Trophy { get; set; }
}