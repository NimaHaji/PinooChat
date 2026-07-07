namespace Application.Features.Session.Interfaces;

public interface UserSessionServiceContract
{
    Task<string> GenerateSessionToken();
}