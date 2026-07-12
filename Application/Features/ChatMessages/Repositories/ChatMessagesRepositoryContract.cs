using Domain.Entities;

namespace Application.Features.ChatMessages.Repositories;

public interface ChatMessagesRepositoryContract
{
    Task<ChatMessage> AddMessageAsync(ChatMessage chatMessage);
    Task<ChatMessage?> GetByIdAsync(Guid id);
    Task<IEnumerable<ChatMessage>> GetUnseenMessagesAsync(Guid userId);
    Task<int> GetUnseenMessagesCountAsync(Guid userId);
    Task<List<ChatMessage>> GetChatHistoryAsync(Guid user1, Guid user2, int limit = 50);
    Task UpdateStatusBulkAsync(List<Guid> validIds, MessageStatus source);
}