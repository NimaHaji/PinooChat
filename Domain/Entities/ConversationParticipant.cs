namespace Domain.Entities;

public class ConversationParticipant
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public Guid UserId { get; private set; }
    public ParticipantRole ParticipantRole { get; set; }
    public int UnreadMessagesCount { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }

    public Conversation Conversation { get; private set; } = null!;
    public User User { get; private set; }

    private ConversationParticipant()
    {
    }

    public ConversationParticipant(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        JoinedAt = DateTimeOffset.UtcNow;
        UnreadMessagesCount = 0;
    }

    public void IncrementUnread()
    {
        UnreadMessagesCount++;
    }

    public void ResetUnread()
    {
        UnreadMessagesCount = 0;
    }
}

public enum ParticipantRole
{
    Owner,
    Admin,
    Member
}