namespace Application.Features.Group.DTOs;

public class SendGroupMessageDto
{
    public List<Guid> MemberIds { get; set; }
    public Guid? ReplyToId { get; set; }
    public string Content { get; set; } = null!;
}