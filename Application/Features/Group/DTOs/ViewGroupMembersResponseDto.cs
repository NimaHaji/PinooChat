namespace Application.Features.Group.DTOs;

public class ViewGroupMembersResponseDto
{
    public Guid UserId { get; set; }
    public string UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string UserName { get; set; }
}