namespace IvanWeb.Application.Models.Events;

public class StorePurchaseEvent
{
    public int TotalItemsPurchased { get; }
    public int TotalPointsSpent { get; }

    public StorePurchaseEvent(int totalItemsPurchased, int totalPointsSpent)
    {
        TotalItemsPurchased = totalItemsPurchased;
        TotalPointsSpent = totalPointsSpent;
    }
}