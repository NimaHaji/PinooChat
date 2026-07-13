using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Application.Features.Conversation.Interfaces;
using Domain.Entities;

namespace Application.Features.ChatMessages.Implement;

public class ChatMessageService : ChatMessageServiceContract
{
    private readonly ChatMessagesRepositoryContract _chatRepository;
    private readonly ConversationRepositoryContract _conversationRepository;
    private readonly UserRepositoryContract _userRepository;
    private readonly UnitOfWorkContract _unitOfWorkContract;

    public ChatMessageService(
        ChatMessagesRepositoryContract chatRepository,
        UnitOfWorkContract unitOfWorkContract,
        UserRepositoryContract userRepository,
        ConversationRepositoryContract conversationRepository)
    {
        _chatRepository = chatRepository;
        _unitOfWorkContract = unitOfWorkContract;
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<SentChatMessageDto> SendMessageAsync(SendMessageDto dto)
    {
        if (dto.SenderId == Guid.Empty)
            throw new ValidationException("شناسه فرستنده نامعتبر است.");

        if (dto.ReceiverId == Guid.Empty)
            throw new ValidationException("شناسه گیرنده نامعتبر است.");

        if (dto.SenderId == dto.ReceiverId)
            throw new ValidationException("ارسال پیام به خودتان مجاز نیست.");

        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ValidationException("پیام نمی‌تواند خالی باشد.");

        if (dto.Content.Length > 1000)
            throw new ValidationException("پیام بسیار طولانی است.");

        var normalizedContent = dto.Content.Trim();

        var conversation = await _conversationRepository.GetConversationByParticipantIds(dto.SenderId, dto.ReceiverId);

        if (conversation is null)
        {
            conversation = Domain.Entities.Conversation.Create(dto.SenderId, dto.ReceiverId);
            await _conversationRepository.AddConversationAsync(conversation);
        }

        var message = new ChatMessage(
            conversationId: conversation.Id,
            senderId: dto.SenderId,
            receiverId: dto.ReceiverId,
            content: normalizedContent
        );

        await _chatRepository.AddMessageAsync(message);

        conversation.UpdateLastMessage(message.Content);

        var receiverParticipant = conversation.GetParticipant(dto.ReceiverId);
        receiverParticipant?.IncrementUnread();

        await _unitOfWorkContract.SaveAsync();

        var sender = await _userRepository.GetUserByIdAsync(dto.SenderId);
        var receiver = await _userRepository.GetUserByIdAsync(dto.ReceiverId);
        
        return new SentChatMessageDto
        {
            Id = message.Id,
            ConversationId = conversation.Id,
            SenderId = message.SenderId,
            SenderName = $"{sender?.FirstName} {sender?.LastName}".Trim(),
            ReceiverId = message.ReceiverId,
            ReceiverName = $"{receiver?.FirstName} {receiver?.LastName}".Trim(),
            Content = message.Content,
            TimeStamp = message.TimeStamp
        };
    }

    public async Task MarkMessageAsDeliveredAsync(Guid messageId)
    {
        if (messageId == Guid.Empty)
            throw new ValidationException("شناسه پیام نامعتبر است.");

        var message = await _chatRepository.GetByIdAsync(messageId);

        if (message is null)
            throw new KeyNotFoundException("پیام یافت نشد.");

        message.MarkMessageAsDelivered();

        await _unitOfWorkContract.SaveAsync();
    }

    public async Task MarkMessagesAsSeenAsync(Guid userId, List<Guid> messageIds)
    {
        if (userId == Guid.Empty)
            throw new ValidationException("شناسه کاربر نامعتبر است.");

        if (messageIds is null || !messageIds.Any())
            return;

        var ids = messageIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (!ids.Any())
            return;

        var messages = await _chatRepository.GetUnseenMessagesAsync(userId);

        var validMessages = messages
            .Where(m => ids.Contains(m.Id))
            .ToList();

        if (!validMessages.Any())
            return;

        var validIds = validMessages.Select(m => m.Id).ToList();

        await _chatRepository.UpdateStatusBulkAsync(validIds, MessageStatus.Seen);

        var conversationIds = validMessages
            .Select(m => m.ConversationId)
            .Distinct()
            .ToList();

        foreach (var conversationId in conversationIds)
        {
            var conversation = await _conversationRepository.GetConversationByIdWithParticipant(conversationId);

            var participant = conversation?.GetParticipant(userId);
            participant?.ResetUnread();
        }

        await _unitOfWorkContract.SaveAsync();
    }
}
