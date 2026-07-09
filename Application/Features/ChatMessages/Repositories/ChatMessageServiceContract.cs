using Application.Features.ChatMessages.DTOs;
using Domain.Entities;

namespace Application.Features.ChatMessages.Repositories;

public interface ChatMessageServiceContract
{
    Task<SentChatMessageDto> SendMessageAsync(SendMessageDto dto);
}