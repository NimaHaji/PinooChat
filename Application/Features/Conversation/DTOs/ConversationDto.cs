namespace Application.Features.Conversation.DTOs;

public class ConversationDto
{
    public Guid ConversationId { get; set; }
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; }
    public string? LastMessage { get; set; }
    public DateTimeOffset? LastMessageTime { get; set; }
    public int UnSeenCount { get; set; }
}