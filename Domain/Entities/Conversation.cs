namespace Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; }
    public string? LastMessage { get; private set; }
    public DateTimeOffset LastMessageAt { get; private set; }

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<ChatMessage> Messages { get; private set; } = new List<ChatMessage>();

    private Conversation()
    {
    }

    private Conversation(IEnumerable<ConversationParticipant> participants)
    {
        Id = Guid.NewGuid();
        LastMessageAt = DateTimeOffset.UtcNow;
        Participants = participants.ToList();
    }

    public static Conversation Create(Guid firstUserId, Guid secondUserId)
    {
        if (firstUserId == Guid.Empty)
            throw new ArgumentException("First user id is required.", nameof(firstUserId));

        if (secondUserId == Guid.Empty)
            throw new ArgumentException("Second user id is required.", nameof(secondUserId));

        if (firstUserId == secondUserId)
            throw new InvalidOperationException("Cannot create conversation with the same user.");

        var participants = new List<ConversationParticipant>
        {
            new ConversationParticipant(firstUserId),
            new ConversationParticipant(secondUserId)
        };

        return new Conversation(participants);
    }

    public static Conversation Create(IEnumerable<ConversationParticipant> participants)
    {
        var participantList = participants.ToList();

        if (participantList.Count < 2)
            throw new InvalidOperationException("Conversation must have at least two participants.");

        return new Conversation(participantList);
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
