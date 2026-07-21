using IvanWeb.Domain.Enums;

namespace IvanWeb.Domain.Entities;

public class Trophy : EntityBase
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Points { get; set; }

    public TrophyCategory Category { get; set; }
    public TrophyRarity Rarity { get; set; }

    public bool IsSecret { get; set; }
    public string? SecretHint { get; set; }
}