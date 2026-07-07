using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities;

public class UserSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public string AccessTokenHash { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiredAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public DateTime LastSeenAtUtc { get; private set; }
    public User User { get; private set; } = null!;
    public string? DeviceName { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public Guid? ReplacedBySessionId { get; private set; }
    public string? RevokedReason { get; private set; }

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsExpired => DateTime.UtcNow >= ExpiredAtUtc;

    public bool IsActive => !IsRevoked && !IsExpired;

    public UserSession(Guid userId, string tokenHash, DateTime expiredAtUtc, string deviceName, string ipAddress,
        string userAgent, string accessTokenHash)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        AccessTokenHash = accessTokenHash;
        ExpiredAtUtc = expiredAtUtc;
        DeviceName = deviceName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateAccessToken(string newAccessTokenHash)
    {
        AccessTokenHash = newAccessTokenHash;   
    }
    public void Revoke(Guid? replacedBySessionId, string revokedReason)
    {
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedBySessionId = replacedBySessionId;
        RevokedReason = revokedReason;
    }

    public void UpdateLastSeen()
    {
        LastSeenAtUtc = DateTime.UtcNow;
    }
}