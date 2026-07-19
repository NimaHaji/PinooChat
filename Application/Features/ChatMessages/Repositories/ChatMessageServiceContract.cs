using Application.Features.ChatMessages.DTOs;
using Application.Features.Group.DTOs;
using Domain.Entities;

namespace Application.Features.ChatMessages.Repositories;

public interface ChatMessageServiceContract
{
    Task<SentChatMessageDto> SendMessageAsync(SendMessageDto dto);
    Task<GroupMessageResultDto> SendGroupMessageAsync(Guid SenderId,SendGroupMessageDto dto);
    Task MarkMessageAsDeliveredAsync(Guid messageId);
    Task MarkMessagesAsSeenAsync(Guid userId, List<Guid> messageIds);
}