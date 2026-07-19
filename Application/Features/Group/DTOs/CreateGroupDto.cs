namespace Application.Features.Group.DTOs;

public class CreateGroupDto
{
    public string GroupTitle { get; set; }
    public string GroupIdName { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public List<Guid> MemberIds { get; set; } = [];
}