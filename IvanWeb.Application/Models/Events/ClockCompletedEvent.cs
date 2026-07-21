namespace IvanWeb.Application.Models.Events;

public class ClockCompletedEvent
{
    public int CorrectCount { get; }
    public int IncorrectCount { get; }
    public int TotalAnswered => CorrectCount + IncorrectCount;

    public ClockCompletedEvent(int correctCount, int incorrectCount)
    {
        CorrectCount = correctCount;
        IncorrectCount = incorrectCount;
    }
}