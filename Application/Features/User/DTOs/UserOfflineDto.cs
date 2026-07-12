namespace Application.Features.User.DTOs;

public class UserOfflineDto
{
    public bool IsUserCompletelyOffline { get; set; }
    public string? UserId { get; set; }
    public long RemainingConnections { get; set; }
}