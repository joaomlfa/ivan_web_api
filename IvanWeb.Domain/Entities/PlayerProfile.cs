namespace IvanWeb.Domain.Entities;

public class PlayerProfile : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public int TotalScore { get; set; } = 0;
    public DateTime LastLoginAt { get; set; }

    // Estatísticas do Quiz Clássico
    public int QuizRuns { get; set; }
    public int QuizCorrectAnswers { get; set; }
    public int QuizIncorrectAnswers { get; set; }
    public DateTime LastQuizPlayedDate { get; set; }

    // Contadores do Jogo Musical (Antigo AppState)
    public int QuizSongsRuns { get; set; }
    public int QuizSongsCorrectAnswers { get; set; }
    public int QuizSongsIncorrectAnswers { get; set; }

    // Sistema de Limite Diário
    public DateTime LastPlayedDate { get; set; } = DateTime.UtcNow;
    public int Daily8BitRuns { get; set; }
    public int DailyHardRuns { get; set; }
    public int DailyClockRuns { get; set; }
    public DateTime FirstRunDate { get; set; }
    public int RunCount { get; set; }

    // Propriedades de Navegação (Relacionamentos)
    public ICollection<UnlockedTrophy> UnlockedTrophies { get; set; } = new List<UnlockedTrophy>();
    public ICollection<PurchaseHistory> Purchases { get; set; } = new List<PurchaseHistory>();

    // A nova lista de músicas inéditas
    public ICollection<Song> GuessedSongs { get; set; } = new List<Song>();

}