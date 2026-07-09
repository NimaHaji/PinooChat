using Domain.Entities;

namespace Application.Features.ChatMessages.Repositories;

public interface ChatMessagesRepositoryContract
{
    Task SendMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetChatHistoryAsync(Guid user1, Guid user2, int limit = 50);
}