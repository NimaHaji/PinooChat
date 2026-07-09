using System.Text.Json;
using Application.Features.ChatMessages.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Repositories.Chat;

public class ChatMessageRepository:ChatMessagesRepositoryContract
{
    private readonly ChatContext _dbContext;

    public ChatMessageRepository(ChatContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SendMessageAsync(ChatMessage message)
    {
       await _dbContext.ChatMessages.AddAsync(message);
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

}