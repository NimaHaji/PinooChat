using Application.Features.ChatMessages.DTOs;
using Domain.Entities;

namespace Application.Features.ChatMessages.Repositories;

public interface ChatMessageServiceContract
{
    Task<SentChatMessageDto> SendMessageAsync(SendMessageDto dto);
    Task MarkMessageAsDeliveredAsync(Guid messageId);
    Task MarkMessagesAsSeenAsync(Guid userId, List<Guid> messageIds);
}