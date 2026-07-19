// Domain/Entities/ChatMessage.cs

using Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; private set; }
    public Guid ConversationId { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid? ReceiverId { get; private set; }
    public Guid? ReplyToId { get; private set; }
    public MessageStatus MessageStatus { get; private set; }
    public string Content { get; private set; } = null!;
    public DateTimeOffset? DeliveredAt { get; private set; }
    public DateTimeOffset? SeenAt { get; private set; }
    public DateTimeOffset TimeStamp { get; private set; }

    public Conversation Conversation { get; private set; }
    public User Sender { get; private set; }
    public ChatMessage? ReplyTo { get; private set; }
    public ICollection<ChatMessage> Replies { get; private set; }

    private ChatMessage() { }
    
    public ChatMessage(Guid conversationId, Guid senderId, Guid receiverId, string content, Guid? replyToId = null)
    {
        Id = Guid.NewGuid();
        ConversationId = conversationId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        ReplyToId = replyToId;
        TimeStamp = DateTimeOffset.UtcNow;
        MessageStatus = MessageStatus.Delivered;
    }
    
    public ChatMessage(Guid conversationId, Guid senderId, string content, Guid? replyToId = null)
    {
        Id = Guid.NewGuid();
        ConversationId = conversationId;
        SenderId = senderId;
        ReceiverId = null; 
        Content = content;
        ReplyToId = replyToId;
        TimeStamp = DateTimeOffset.UtcNow;
        MessageStatus = MessageStatus.Delivered;
        DeliveredAt = DateTimeOffset.UtcNow;
    }

    public void MarkMessageAsDelivered()
    {
        MessageStatus = MessageStatus.Delivered;
        DeliveredAt = DateTimeOffset.UtcNow;
    }

    public void MarkMessageAsSeen()
    {
        MessageStatus = MessageStatus.Seen;
        SeenAt = DateTimeOffset.UtcNow;
    }
}