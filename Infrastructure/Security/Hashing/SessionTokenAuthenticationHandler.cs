// Infrastructure/Auth/SessionTokenAuthenticationHandler.cs
using System.Security.Claims;
using System.Text.Encodings.Web;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.Session.Interfaces;
using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class SessionTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UserSessionRepositoryContract _sessionRepository;
    private readonly UserRepositoryContract _userRepository;
    private readonly UnitOfWorkContract _unitOfWorkContract; 
    private readonly IHasher _hasher;

    public SessionTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        UserSessionRepositoryContract sessionRepository,
        IHasher hasher, UnitOfWorkContract unitOfWorkContract, UserRepositoryContract userRepository)
        : base(options, logger, encoder, clock)
    {
        _sessionRepository = sessionRepository;
        _hasher = hasher;
        _unitOfWorkContract = unitOfWorkContract;
        _userRepository = userRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // ۱. گرفتن توکن از هدر
        string? token = null;
        
        // روش اول: از هدر Authorization (مثل JWT)
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader["Bearer ".Length..].Trim();
        }
        
        if (string.IsNullOrEmpty(token))
        {
            token = Request.Headers["X-Session-Token"].FirstOrDefault();
        }
        
        if (string.IsNullOrEmpty(token))
        {
            token = Request.Query["access_token"].FirstOrDefault();
        }

        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.NoResult();
        }

        try
        {

            var tokenHash = _hasher.Hash(token);
            var session = await _sessionRepository.GetSessionByAccessTokenHashedAsync(tokenHash);

            
            if (session == null || !session.IsActive)
            {
                return AuthenticateResult.Fail("Invalid or expired session token");
            }
            
            session.UpdateLastSeen();
            await _sessionRepository.UpdateAsync(session);
            await _unitOfWorkContract.SaveAsync();
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, session.UserId.ToString()),
                new(ClaimTypes.Sid, session.Id.ToString()),
                new("device", session.DeviceName ?? "Unknown"),
                new("ip_address", session.IpAddress ?? "Unknown"),
            };
            
            var user = await _userRepository.GetUserByIdAsync(session.UserId);
            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid session user");
            }

            claims.Add(new(ClaimTypes.Role, user.UserRole.ToString()));
            claims.Add(new(ClaimTypes.Email, user.Email));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Session token authentication failed");
            return AuthenticateResult.Fail("Authentication failed");
        }
    }
}
