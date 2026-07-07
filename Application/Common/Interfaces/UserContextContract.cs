namespace Application.Common.Interfaces;

public interface UserContextContract
{
    Guid? UserId { get;}
    string? Email { get;}
    bool IsAuthenticated { get;}
}