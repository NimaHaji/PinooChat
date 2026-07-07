using System.Security.Claims;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Persistence.Contexts;

public class UserContext : UserContextContract
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id)
                ? id
                : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor
        .HttpContext?
        .User?
        .Identity?
        .IsAuthenticated ?? false;

    public string? Email => _httpContextAccessor.HttpContext?
        .User?
        .FindFirst(ClaimTypes.Email)?.Value;
    
}
