using IvanWeb.Domain.Enums;

namespace IvanWeb.Domain.Entities;

public class Song : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int Year { get; set; }
    public SongsGenre Genre { get; set; }
    public int Points { get; set; }
    public bool Is8Bit { get; set; }

    // Este é o nome do arquivo físico no seu servidor sem extensão (ex: d3a2b1c4-1a2b)
    public string FileId { get; set; } = string.Empty;
}