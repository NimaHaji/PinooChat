using Application.Features.Group.DTOs;
using Application.Features.Group.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Chat;

public class GroupRepository:GroupRepositoryContract
{
    private readonly ChatContext _context;

    public GroupRepository(ChatContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByIdNameAsync(string groupIdName)
    {
        return await _context
            .Groups
            .Where(g => g.GroupIdName == groupIdName)
            .AnyAsync();
    }

    public async Task AddGroupAsync(Group group)
    {
        await _context
            .Groups
            .AddAsync(group);
    }

    public async Task<List<ViewGroupMembersResponseDto>> GetMembersAsync(Guid conversationId)
    {
        return await _context.Groups
            .Where(g => g.ConversationId == conversationId)
            .SelectMany(g => g.Conversation.Participants)
            .Select(p => new ViewGroupMembersResponseDto
            {
                UserId = p.UserId,
                UserFirstName = p.User.FirstName,
                UserLastName = p.User.LastName,
                UserName = p.User.UserName
            })
            .ToListAsync();
    }
}