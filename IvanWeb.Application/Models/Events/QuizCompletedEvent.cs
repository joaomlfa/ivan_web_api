using IvanWeb.Domain.Enums;

namespace IvanWeb.Application.Models.Events;

public class QuizCompletedEvent
{
    public Guid QuestionId { get; set; }
    public bool IsCorrect { get; set; }
    public QuestionType Type { get; set; } // O Enum QuestionType que criamos no Domain
    public int PointsEarned { get; set; }

    // Construtor para facilitar a criação do evento
    public QuizCompletedEvent(Guid questionId, bool isCorrect, QuestionType type, int pointsEarned)
    {
        QuestionId = questionId;
        IsCorrect = isCorrect;
        Type = type;
        PointsEarned = pointsEarned;
    }
}