using Application.Features.Session.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.User;

public class UserSessionRepository : UserSessionRepositoryContract
{
    private readonly ChatContext _context;

    public UserSessionRepository(ChatContext context)
    {
        _context = context;
    }

    public async Task CreateUserSessionAsync(UserSession userSession)
    {
        await _context
            .UserSessions
            .AddAsync(userSession);
    }

    public async Task<UserSession?> GetSessionByAccessTokenHashedAsync(string hashedToken)
    {
        return await _context
            .UserSessions
            .Where(ss => ss.AccessTokenHash == hashedToken)
            .FirstOrDefaultAsync();
    }

    public async Task<UserSession?> GetSessionByRefreshTokenHashedAsync(string hashedToken)
    {
        return await _context
            .UserSessions
            .Where(ss => ss.TokenHash == hashedToken)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserSession>?> GetSessionsByUserIdAsync(Guid userId)
    {
        return await _context
            .UserSessions
            .Where(ss => ss.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateAsync(UserSession session)
    {
        _context
            .UserSessions
            .Update(session);
    }
}