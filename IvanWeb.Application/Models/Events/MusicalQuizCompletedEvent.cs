namespace IvanWeb.Application.Models.Events;

public class MusicalQuizCompletedEvent
{
    public Guid SongId { get; set; }
    public bool IsCorrect { get; set; }
    public double TimeTakenSeconds { get; set; }

    public MusicalQuizCompletedEvent(Guid songId, bool isCorrect, double timeTakenSeconds)
    {
        SongId = songId;
        IsCorrect = isCorrect;
        TimeTakenSeconds = timeTakenSeconds;
    }
}