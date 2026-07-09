using Application.Common.Interfaces;
using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Domain.Entities;

namespace Application.Features.ChatMessages.Implement;

public class ChatMessageService:ChatMessageServiceContract
{
    private readonly ChatMessagesRepositoryContract _chatRepository;
    private readonly UnitOfWorkContract _unitOfWorkContract;
    public ChatMessageService(ChatMessagesRepositoryContract chatRepository, UnitOfWorkContract unitOfWorkContract)
    {
        _chatRepository = chatRepository;
        _unitOfWorkContract = unitOfWorkContract;
    }

    public async Task<SentChatMessageDto> SendMessageAsync(SendMessageDto dto)
    {
        var message = new ChatMessage(
            senderId: dto.SenderId,
            receiverId: dto.ReceiverId,
            content: dto.Content
            );
        
        await _chatRepository.SendMessageAsync(message);
        await _unitOfWorkContract.SaveAsync();

        return new SentChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            TimeStamp = message.TimeStamp
        };
    }
}