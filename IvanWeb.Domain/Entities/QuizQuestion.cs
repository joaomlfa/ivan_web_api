namespace IvanWeb.Domain.Entities;

public class QuizQuestion
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int Points { get; set; }
    public bool IsAnswered { get; set; }
    public DateTime CreatedAt { get; set; }
}