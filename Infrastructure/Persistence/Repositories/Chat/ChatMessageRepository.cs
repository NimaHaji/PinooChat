using System.Text.Json;
using Application.Features.ChatMessages.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Repositories.Chat;

public class ChatMessageRepository : ChatMessagesRepositoryContract
{
    private readonly ChatContext _dbContext;

    public ChatMessageRepository(ChatContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChatMessage> AddMessageAsync(ChatMessage chatMessage)
    {
        await _dbContext.ChatMessages.AddAsync(chatMessage);
        return chatMessage;
    }

    public async Task<ChatMessage?> GetByIdAsync(Guid id)
    {
        return await _dbContext.ChatMessages.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<ChatMessage>> GetUnseenMessagesAsync(Guid userId)
    {
        return await _dbContext
            .ChatMessages
            .Where(msg => msg.ReceiverId == userId && msg.MessageStatus != MessageStatus.Seen)
            .OrderBy(x => x.TimeStamp)
            .ToListAsync();
    }

    public async Task<int> GetUnseenMessagesCountAsync(Guid userId)
    {
        return await _dbContext
            .ChatMessages
            .CountAsync(msg => msg.SenderId == userId && msg.MessageStatus != MessageStatus.Seen);
    }

    public async Task<List<ChatMessage>> GetChatHistoryAsync(Guid user1, Guid user2, int limit = 50)
    {
        return await _dbContext.ChatMessages
            .Where(x =>
                (x.SenderId == user1 && x.ReceiverId == user2) ||
                (x.SenderId == user2 && x.ReceiverId == user1))
            .OrderByDescending(x => x.TimeStamp)
            .Take(limit)
            .OrderBy(x => x.TimeStamp)
            .ToListAsync();
    }

    public async Task UpdateStatusBulkAsync(List<Guid> messageIds, MessageStatus status)
    {
        var ids = messageIds.ToList();
        if (!ids.Any()) return;

        await _dbContext.ChatMessages
            .Where(m => ids.Contains(m.Id))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.MessageStatus, status)
                .SetProperty(
                    m => m.DeliveredAt,
                    m => status == MessageStatus.Delivered ? DateTimeOffset.UtcNow : m.DeliveredAt
                )
                .SetProperty(
                    m => m.SeenAt,
                    m => status == MessageStatus.Seen ? DateTimeOffset.UtcNow : m.SeenAt
                )
            );
    }
}