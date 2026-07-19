using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string Email { get; private set; }
    public string? MobilePhone { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserRole UserRole { get; set; }
    public string? PasswordResetCodeHash { get; private set; }
    public DateTime? PasswordResetCodeExpireAt { get; private set; }
    public int PasswordResetAttemptsCount { get; private set; }
    public ICollection<UserSession> Sessions { get; private set; } = new List<UserSession>();

    public List<ConversationParticipant> ConversationParticipants { get; private set; } =
        new List<ConversationParticipant>();

    public List<ChatMessage> ChatMessages { get; private set; } = new List<ChatMessage>();

    public User(string userName, string firstName, string? lastName, string? email, string? mobilePhone,
        string passwordHash)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        MobilePhone = mobilePhone;
        PasswordHash = passwordHash;
        UserRole = UserRole.User;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole role)
    {
        UserRole = role;
    }

    public void UpdateProfile(string? firstName, string? lastName, string? mobilePhone)
    {
        FirstName = firstName;
        LastName = lastName;
        MobilePhone = mobilePhone;
    }

    public void ResetPassword(string codeHash, DateTime expiresAt)
    {
        PasswordResetCodeHash = codeHash;
        PasswordResetCodeExpireAt = expiresAt;
        PasswordResetAttemptsCount = 0;
    }

    public bool CanUseResetPassword(string codeHash, DateTime now)
    {
        if (PasswordResetCodeExpireAt == null || now > PasswordResetCodeExpireAt)
            return false;

        return PasswordResetCodeHash == codeHash;
    }

    public void IncreasePasswordResetAttemptCount() => PasswordResetAttemptsCount++;

    public void ClearPasswordResetCode()
    {
        PasswordResetCodeHash = null;
        PasswordResetCodeExpireAt = null;
        PasswordResetAttemptsCount = 0;
    }

    public void ChangeFirstName(string firstName)
    {
        FirstName = firstName;
    }

    public void ChangeLastName(string lastName)
    {
        LastName = lastName;
    }

    public void ChangeMobilePhone(string mobilePhone)
    {
        MobilePhone = mobilePhone;
    }
}