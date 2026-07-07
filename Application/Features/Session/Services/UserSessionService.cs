using System.Security.Cryptography;
using Application.Features.Session.Interfaces;

namespace Application.Features.Session.Services;

public class UserSessionService:UserSessionServiceContract
{
    public async Task<string> GenerateSessionToken()
    {
        var random = new byte[64];
        using var rng=RandomNumberGenerator.Create();
        rng.GetBytes(random);
        return Convert.ToBase64String(random);
    }
}