namespace Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; } 
    public string Content { get; private set; } = null!;
    public DateTimeOffset TimeStamp { get; private set; }

    public ChatMessage(Guid senderId, Guid receiverId, string content)
    {
        Id = Guid.NewGuid();
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        TimeStamp = DateTimeOffset.UtcNow;
    }
}