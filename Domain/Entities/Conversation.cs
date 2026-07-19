using Domain.Enums;

namespace Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; }
    public string? LastMessage { get; private set; }
    public DateTimeOffset? LastMessageAt { get; private set; }
    public ConversationType ConversationType { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public ICollection<ConversationParticipant> Participants { get; private set; } =
        new List<ConversationParticipant>();

    public ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

    public Group Group { get; private set; }

    private Conversation()
    {
    }

    private Conversation(
        IEnumerable<ConversationParticipant> participants,
        ConversationType conversationType)
    {
        Id = Guid.NewGuid();
        Participants = participants.ToList();
        ConversationType = conversationType;
        CreatedAt = DateTimeOffset.UtcNow;
        LastMessageAt = CreatedAt;
    }

    public static Conversation CreatePrivate(Guid firstUserId, Guid secondUserId)
    {
        if (firstUserId == Guid.Empty)
            throw new ArgumentException(nameof(firstUserId));

        if (secondUserId == Guid.Empty)
            throw new ArgumentException(nameof(secondUserId));

        if (firstUserId == secondUserId)
            throw new InvalidOperationException("Cannot create conversation with the same user.");

        var participants = new List<ConversationParticipant>
        {
            new(firstUserId),
            new(secondUserId)
        };

        return new Conversation(participants, ConversationType.Private);
    }

    public static Conversation CreateGroup(IEnumerable<ConversationParticipant> participants)
    {
        var participantList = participants.ToList();

        if (participantList.Count < 2)
            throw new InvalidOperationException("Conversation must have at least two participants.");

        return new Conversation(participantList, ConversationType.Group);
    }

    public void UpdateLastMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        LastMessage = message;
        LastMessageAt = DateTimeOffset.UtcNow;
    }

    public ConversationParticipant? GetParticipant(Guid userId)
    {
        return Participants.FirstOrDefault(p => p.UserId == userId);
    }
}