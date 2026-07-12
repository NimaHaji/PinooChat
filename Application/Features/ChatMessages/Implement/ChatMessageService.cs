using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Domain.Entities;

namespace Application.Features.ChatMessages.Implement;

public class ChatMessageService : ChatMessageServiceContract
{
    private readonly ChatMessagesRepositoryContract _chatRepository;
    private readonly UserRepositoryContract _userRepository;
    private readonly UnitOfWorkContract _unitOfWorkContract;

    public ChatMessageService(ChatMessagesRepositoryContract chatRepository, UnitOfWorkContract unitOfWorkContract,
        UserRepositoryContract userRepository)
    {
        _chatRepository = chatRepository;
        _unitOfWorkContract = unitOfWorkContract;
        _userRepository = userRepository;
    }

    public async Task<SentChatMessageDto> SendMessageAsync(SendMessageDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ValidationException("پیام نمیتواند خالی باشد .");

        if (dto.Content.Length > 1000)
            throw new ValidationException("پیام بسیار طولانی است .");

        var message = new ChatMessage(
            senderId: dto.SenderId,
            receiverId: dto.ReceiverId,
            content: dto.Content
        );

        await _chatRepository.AddMessageAsync(message);
        await _unitOfWorkContract.SaveAsync();

        var sender = await _userRepository.GetUserByIdAsync(dto.SenderId);
        var receiver = await _userRepository.GetUserByIdAsync(dto.ReceiverId);

        return new SentChatMessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = sender?.FirstName + " " + sender?.LastName,
            ReceiverId = message.ReceiverId,
            ReceiverName = receiver?.FirstName + " " + receiver?.LastName,
            Content = message.Content,
            TimeStamp = message.TimeStamp
        };
    }

    public async Task MarkMessageAsDeliveredAsync(Guid messageId)
    {
        var message=await _chatRepository.GetByIdAsync(messageId);
        message.MarkMessageAsDelivered();
        await _unitOfWorkContract.SaveAsync();
    }

    public async Task MarkMessagesAsSeenAsync(Guid userId, List<Guid> messageIds)
    {
        var ids = messageIds.ToList();
        if (!ids.Any()) return;
        
        var messages = await _chatRepository.GetUnseenMessagesAsync(userId);
        var validIds = messages.Select(m => m.Id).Intersect(ids).ToList();
        
        if (validIds.Any())
        {
            await _chatRepository.UpdateStatusBulkAsync(validIds, MessageStatus.Seen);
            await _unitOfWorkContract.SaveAsync();

            var messageIdsBySender = messages
                .Where(m => validIds.Contains(m.Id))
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, MessageIds = g.Select(m => m.Id) });

            // foreach (var group in messageIdsBySender)
            // {
            //     await _cacheService.PublishAsync($"messages-Seen-{group.SenderId}", 
            //         JsonSerializer.Serialize(new { 
            //             UserId = userId, 
            //             MessageIds = group.MessageIds 
            //         }));
            // }
        }
    }
}