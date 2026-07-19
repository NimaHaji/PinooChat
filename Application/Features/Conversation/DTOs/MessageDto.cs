namespace Application.Features.Conversation.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public string Content { get; set; } = null!;
    public DateTimeOffset TimeStamp { get; set; }
}