using Application.Features.ChatMessages.DTOs;
using Application.Features.Conversation.DTOs;

namespace Application.Features.Conversation.Interfaces;

public interface ConversationServiceContract
{
    Task<Domain.Entities.Conversation> GetOrCreateConversationAsync(Guid senderId, Guid receiverId);
    Task<List<ConversationDto>> GetConversationsAsync();
    Task<List<MessageDto>?> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize);
}