using Application.Features.Group.DTOs;

namespace Application.Features.Group.Interfaces;

public interface GroupServiceContract
{
    Task CreateGroup(CreateGroupDto dto);
    Task<List<ViewGroupMembersResponseDto>> GetMembers(Guid conversationId);
}