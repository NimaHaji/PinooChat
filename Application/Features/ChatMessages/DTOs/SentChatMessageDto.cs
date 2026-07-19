namespace Application.Features.ChatMessages.DTOs;

public class SentChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public string Content { get; set; } = null!;
    public DateTimeOffset TimeStamp { get; set; }

    public ReplyToMessagePreview? ReplyTo { get; set; }
}

public class ReplyToMessagePreview
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = null!;
}