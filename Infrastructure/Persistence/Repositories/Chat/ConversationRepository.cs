using Application.Features.Conversation.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Chat;

public class ConversationRepository:ConversationRepositoryContract
{
    private readonly ChatContext _chatContext;

    public ConversationRepository(ChatContext chatContext)
    {
        _chatContext = chatContext;
    }

    public async Task<Conversation?> GetConversationByParticipantIds(Guid senderId, Guid receiverId)
    {
        return await _chatContext
            .ConversationParticipants
            .Where(p => p.UserId == senderId || p.UserId == receiverId)
            .GroupBy(p => p.Conversation)
            .Where(g => g.Count() == 2)
            .Select(g => g.Key)
            .FirstOrDefaultAsync();
    }

    public async Task<Conversation?> GetGroupByMembersAsync(Guid senderId,List<Guid> membersIds)
    {
        var allMemberIds = membersIds
            .Concat(new[] { senderId })
            .Distinct()
            .ToList();
    
        return await _chatContext.Conversations
            .AsNoTracking()
            .Where(c => c.ConversationType == ConversationType.Group)
            .Where(c => c.Participants.Count == allMemberIds.Count)
            .Where(c => allMemberIds.All(id => c.Participants.Any(p => p.UserId == id)))
            .Include(c => c.Group)
            .Include(c => c.Participants)
            .FirstOrDefaultAsync();
    }

    public async Task AddConversationAsync(Conversation conversation)
    {
        await _chatContext
            .Conversations
            .AddAsync(conversation);
    }

    public async Task<Conversation?> GetConversationByIdWithParticipant(Guid conversationId)
    {
        return await _chatContext
            .Conversations
            .Include(c => c.Participants)
            .Where(c => c.Id == conversationId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Conversation?>> GetConversationsByUserId(Guid userId)
    {
        return await _chatContext
            .Conversations
            .Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();
    }
    
    public async Task<List<ChatMessage>> GetConversationMessagesAsync(
        Guid conversationId,
        int page,
        int pageSize)
    {
        return await _chatContext.ChatMessages
            .Where(x => x.ConversationId == conversationId)
            .OrderByDescending(x => x.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

}