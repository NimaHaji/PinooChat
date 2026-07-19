using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Application.Features.Conversation.Interfaces;
using Application.Features.Group.DTOs;
using Domain.Entities;
using Shared.Exceptions;

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
        var normalizedContent = dto.Content.Trim();

        var conversation = await _conversationRepository
            .GetConversationByParticipantIds(dto.SenderId, dto.ReceiverId);

        if (conversation is null)
        {
            conversation = Domain.Entities.Conversation.CreatePrivate(dto.SenderId, dto.ReceiverId);
            await _conversationRepository.AddConversationAsync(conversation);
        }

        ChatMessage? replyMessage = null;

        if (dto.ReplyToId.HasValue)
        {
            replyMessage = await _chatRepository.GetByIdAsync(dto.ReplyToId.Value);

            if (replyMessage is null)
                throw new NotFoundException("پیام مورد نظر یافت نشد.");

            if (replyMessage.ConversationId != conversation.Id)
                throw new ValidationException("پیام انتخاب شده متعلق به این گفتگو نیست.");
        }

        var message = new ChatMessage(
            conversationId: conversation.Id,
            senderId: dto.SenderId,
            receiverId: dto.ReceiverId,
            content: normalizedContent,
            replyToId: dto.ReplyToId
        );

        await _chatRepository.AddMessageAsync(message);

        conversation.UpdateLastMessage(message.Content);

        var receiverParticipant = conversation.GetParticipant(dto.ReceiverId);
        receiverParticipant?.IncrementUnread();

        await _unitOfWorkContract.SaveAsync();

        return new SentChatMessageDto
        {
            Id = message.Id,
            ConversationId = conversation.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            TimeStamp = message.TimeStamp,

            ReplyTo = replyMessage == null
                ? null
                : new ReplyToMessagePreview
                {
                    Id = replyMessage.Id,
                    SenderId = replyMessage.SenderId,
                    Content = replyMessage.Content
                }
        };
    }

    public async Task<GroupMessageResultDto> SendGroupMessageAsync(Guid senderId,SendGroupMessageDto dto)
    {
        
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ValidationException("Message content cannot be empty");
    
        if (dto.MemberIds == null || !dto.MemberIds.Any())
            throw new ValidationException("Member IDs list cannot be empty");
        
        var conversation = await _conversationRepository.GetGroupByMembersAsync(senderId, dto.MemberIds);

        if (conversation is null)
            throw new ValidationException("Group conversation not found");
        
        var sender = conversation.Participants
            .FirstOrDefault(p => p.UserId == senderId);

        if (sender is null)
            throw new ValidationException("Sender is not a member of this group");
        
        var message = new ChatMessage(
            conversationId: conversation.Id,
            senderId: senderId,
            content: dto.Content,
            replyToId: dto.ReplyToId
        );
        
        await _chatRepository.AddMessageAsync(message);
        
        conversation.UpdateLastMessage(dto.Content);
        
        await _unitOfWorkContract.SaveAsync();
        
        return new GroupMessageResultDto
        {
            Id = message.Id,
            ConversationId = conversation.Id,
            GroupTitle = conversation.Group?.GroupTitle,
            Content = message.Content,
            TimeStamp = message.TimeStamp,
            SenderId = senderId,
            MemberIds = conversation.Participants.Select(p => p.UserId).ToList(),
            IsGroupMessage = true
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
