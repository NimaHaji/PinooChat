namespace Application.Features.Group.DTOs;

public class GroupMessageResultDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string GroupTitle { get; set; }
    public string Content { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public Guid SenderId { get; set; }
    public List<Guid> MemberIds { get; set; }
    public bool IsGroupMessage { get; set; }
}