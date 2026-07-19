using System.Data;
using Application.Common.Interfaces;
using Application.Features.Conversation.Interfaces;
using Application.Features.Group.DTOs;
using Application.Features.Group.Interfaces;
using Domain.Entities;

namespace Application.Features.Group.Implement;

public class GroupService : GroupServiceContract
{
    private readonly GroupRepositoryContract _groupRepository;
    private readonly ConversationRepositoryContract _conversationRepository;
    private readonly UnitOfWorkContract _unitOfWorkContract;

    public GroupService(GroupRepositoryContract groupRepository, ConversationRepositoryContract conversationRepository,
        UnitOfWorkContract unitOfWorkContract)
    {
        _groupRepository = groupRepository;
        _conversationRepository = conversationRepository;
        _unitOfWorkContract = unitOfWorkContract;
    }

    public async Task CreateGroup(CreateGroupDto dto)
    {
        var exists = await _groupRepository.ExistsByIdNameAsync(dto.GroupIdName);

        if (exists)
            throw new DuplicateNameException("این شناسه قبلاً ثبت شده است.");

        var participants = dto.MemberIds
            .Distinct()
            .Append(dto.OwnerId)
            .Distinct()
            .Select(id => new ConversationParticipant(id))
            .ToList();

        var conversation = Domain.Entities.Conversation.CreateGroup(participants);

        var group = new Domain.Entities.Group(
            conversation.Id,
            dto.GroupIdName,
            dto.GroupTitle,
            dto.OwnerId,
            dto.Description
        );

        await _conversationRepository.AddConversationAsync(conversation);
        await _groupRepository.AddGroupAsync(group);

        await _unitOfWorkContract.SaveAsync();
    }

    public async Task<List<ViewGroupMembersResponseDto>> GetMembers(Guid conversationId)
    {
        return await _groupRepository.GetMembersAsync(conversationId);
    }
}