using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

namespace Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public MessageStatus MessageStatus { get;private set; }
    public string Content { get; private set; } = null!;
    public DateTimeOffset DeliveredAt { get;private set; }
    public DateTimeOffset SeenAt { get;private set; }
    public DateTimeOffset TimeStamp { get; private set; }

    public ChatMessage(Guid conversationId, Guid senderId, Guid receiverId, string content)
    {
        Id = Guid.NewGuid();
        ConversationId = conversationId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        TimeStamp = DateTimeOffset.UtcNow;
    }

    public void MarkMessageAsDelivered()
    {
        MessageStatus = MessageStatus.Delivered;
    }
}