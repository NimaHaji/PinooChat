using Domain.Entities;

namespace Application.Features.Session.Interfaces;

public interface UserSessionRepositoryContract
{
    Task CreateUserSessionAsync(UserSession userSession);
    Task<UserSession?> GetSessionByAccessTokenHashedAsync(string hashedToken);
    Task<UserSession?> GetSessionByRefreshTokenHashedAsync(string hashedToken);
    Task<List<UserSession>?> GetSessionsByUserIdAsync(Guid userId);
    Task UpdateAsync(UserSession session);
}