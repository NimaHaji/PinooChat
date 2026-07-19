using Application.Features.Group.DTOs;

namespace Application.Features.Group.Interfaces;

public interface GroupRepositoryContract
{
    Task<bool> ExistsByIdNameAsync(string groupIdName);
    Task AddGroupAsync(Domain.Entities.Group group);
    Task<List<ViewGroupMembersResponseDto>> GetMembersAsync(Guid conversationId);
}