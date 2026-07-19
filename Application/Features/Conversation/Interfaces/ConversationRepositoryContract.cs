using Domain.Entities;

namespace Application.Features.Conversation.Interfaces;

public interface ConversationRepositoryContract
{
    Task<Domain.Entities.Conversation?> GetConversationByParticipantIds(Guid senderId, Guid receiverId);
    Task<Domain.Entities.Conversation?> GetGroupByMembersAsync(Guid sendeId,List<Guid> membersIds);
    Task AddConversationAsync(Domain.Entities.Conversation conversation);
    Task<Domain.Entities.Conversation?> GetConversationByIdWithParticipant(Guid conversationId);
    Task<List<Domain.Entities.Conversation?>> GetConversationsByUserId(Guid userId);
    Task<List<ChatMessage>> GetConversationMessagesAsync(Guid conversationId, int page, int pageSize);
}