using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.Conversation.DTOs;
using Application.Features.Conversation.Interfaces;
using Domain.Entities;

namespace Application.Features.Conversation.Implement;

public class ConversationService : ConversationServiceContract
{
    private readonly ConversationRepositoryContract _conversationRepository;
    private readonly UnitOfWorkContract _unitOfWorkContract;
    private readonly UserContextContract _userContextContract;
    private readonly UserRepositoryContract _userRepositoryContract;

    public ConversationService(ConversationRepositoryContract conversationRepository,
        UnitOfWorkContract unitOfWorkContract, UserContextContract userContextContract, UserRepositoryContract userRepositoryContract)
    {
        _conversationRepository = conversationRepository;
        _unitOfWorkContract = unitOfWorkContract;
        _userContextContract = userContextContract;
        _userRepositoryContract = userRepositoryContract;
    }

    public async Task<Domain.Entities.Conversation> GetOrCreateConversationAsync(Guid senderId, Guid receiverId)
    {
        var conversation = await _conversationRepository.GetConversationByParticipantIds(senderId, receiverId);

        if (conversation is not null)
            return conversation;

        var participants = new List<ConversationParticipant>
        {
            new ConversationParticipant(senderId),
            new ConversationParticipant(receiverId)
        };

        var newConversation = Domain.Entities.Conversation.CreatePrivate(senderId,receiverId);

        await _conversationRepository.AddConversationAsync(newConversation);
        await _unitOfWorkContract.SaveAsync();

        return newConversation;
    }

    public async Task<List<ConversationDto>> GetConversationsAsync()
    {
        var userId = _userContextContract.UserId ?? throw new UnauthorizedAccessException("کاربر یافت نشد.");
    
        var conversations = await _conversationRepository.GetConversationsByUserId(userId);
        
        var otherUserIds = conversations
            .SelectMany(c => c.Participants)
            .Where(p => p.UserId != userId)
            .Select(p => p.UserId)
            .Distinct()
            .ToList();
        
        var users = await _userRepositoryContract.GetUsersByIdsAsync(otherUserIds);
        var userDictionary = users.ToDictionary(u => u.Id);
    
        return conversations.Select(conversation =>
        {
            var otherParticipant = conversation.Participants
                .FirstOrDefault(p => p.UserId != userId);
        
            var currentUserParticipant = conversation.GetParticipant(userId);
            
            var otherUser = otherParticipant != null && userDictionary.ContainsKey(otherParticipant.UserId)
                ? userDictionary[otherParticipant.UserId]
                : null;
        
            return new ConversationDto
            {
                ConversationId = conversation.Id,
                OtherUserId = otherParticipant?.UserId ?? Guid.Empty,
                OtherUserName = otherUser?.UserName ?? "کاربر ناشناس",
                LastMessage = conversation.LastMessage,
                LastMessageTime = conversation.LastMessageAt,
                UnSeenCount = currentUserParticipant?.UnreadMessagesCount ?? 0,
            };
        }).OrderByDescending(c => c.LastMessageTime).ToList();
    }
    public async Task<List<MessageDto>> GetConversationMessagesAsync(
        Guid conversationId,
        int page,
        int pageSize)
    {
        var userId = _userContextContract.UserId ?? throw new UnauthorizedAccessException("کاربر یافت نشد.");
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var conversation = await _conversationRepository.GetConversationByIdWithParticipant(conversationId);

        if (conversation is null)
            throw new KeyNotFoundException("گفتگو یافت نشد.");

        var isParticipant = conversation.Participants.Any(p => p.UserId == userId);
        if (!isParticipant)
            throw new UnauthorizedAccessException("شما به این گفتگو دسترسی ندارید.");

        var messages = await _conversationRepository.GetConversationMessagesAsync(
            conversationId,
            page,
            pageSize);

        return messages.Select(message => new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            TimeStamp = message.TimeStamp
        }).ToList();
    }

}