namespace IvanWeb.Application.Models.Events;

public class AppLoginEvent
{
    public bool IsFirstLogin { get; set; }

    public AppLoginEvent(bool isFirstLogin)
    {
        IsFirstLogin = isFirstLogin;
    }
}