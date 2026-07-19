using System.ComponentModel.DataAnnotations;
using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Application.Features.Group.DTOs;
using Application.Features.User.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hub;

[Authorize]
public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly ChatMessageServiceContract _chatMessageServiceContract;
    private readonly UserOnlineStatusServiceContract _userOnlineStatusServiceContract;

    public ChatHub(
        ChatMessageServiceContract chatMessageServiceContract,
        UserOnlineStatusServiceContract userOnlineStatusService)
    {
        _chatMessageServiceContract = chatMessageServiceContract;
        _userOnlineStatusServiceContract = userOnlineStatusService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (string.IsNullOrWhiteSpace(userId))
        {
            await base.OnConnectedAsync();
            return;
        }

        var connectionId = Context.ConnectionId;

        await _userOnlineStatusServiceContract.MarkUserOnlineAsync(userId, connectionId);

        await Clients.Others.SendAsync("UserOnline", userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;

        var result = await _userOnlineStatusServiceContract.MarkUserOfflineAsync(connectionId);

        if (result.IsUserCompletelyOffline)
        {
            await Clients.Others.SendAsync("UserOffline", result.UserId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task GetOnlineUsers()
    {
        var onlineUsers = await _userOnlineStatusServiceContract.GetOnlineUsersAsync();
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
    }

    public async Task SendPrivateMessage(SendMessageDto messageDto)
    {
        var senderId = GetUserId();

        if (!senderId.HasValue)
        {
            await Clients.Caller.SendAsync("Error", "User not authenticated");
            return;
        }

        if (!Guid.TryParse(messageDto.ReceiverId.ToString(), out var receiverId))
        {
            await Clients.Caller.SendAsync("Error", "Invalid receiver identifier");
            return;
        }

        if (senderId.Value == receiverId)
        {
            await Clients.Caller.SendAsync("Error", "Cannot send message to yourself");
            return;
        }

        try
        {
            var result = await _chatMessageServiceContract.SendMessageAsync(messageDto);

            var receiverConnections =
                await _userOnlineStatusServiceContract.GetUserConnectionsByIdAsync(receiverId.ToString());

            var isReceiverOnline = receiverConnections is not null && receiverConnections.Any();

            if (isReceiverOnline)
            {
                var receiveTasks = receiverConnections.Select(connectionId =>
                    Clients.Client(connectionId).SendAsync("ReceiveMessage", result));

                await Task.WhenAll(receiveTasks);

                await _chatMessageServiceContract.MarkMessageAsDeliveredAsync(result.Id);

                await Clients.Caller.SendAsync("MessageDelivered", new
                {
                    MessageId = result.Id,
                    ReceiverId = receiverId,
                    DeliveredAt = DateTimeOffset.UtcNow
                });
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiverOffline", new
                {
                    ReceiverId = receiverId,
                    MessageId = result.Id,
                    WillBeDeliveredLater = true
                });
            }

            await Clients.Users(new[]
            {
                senderId.Value.ToString(),
                receiverId.ToString()
            }).SendAsync("UpdateConversationList", new
            {
                ConversationId = result.ConversationId,
                LastMessage = result.Content,
                Timestamp = result.TimeStamp,
                SenderId = senderId.Value,
                ReceiverId = receiverId
            });

            await Clients.Caller.SendAsync("MessageSent", result);
        }
        catch (ValidationException ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
        catch (Exception)
        {
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task SendGroupMessage(SendGroupMessageDto messageDto)
    {
        var senderId = GetUserId();
        if (!senderId.HasValue)
        {
            await Clients.Caller.SendAsync("Error", "User not authenticated");
            return;
        }
        
        try
        {
            // 1. تمام منطق توی GroupService انجام میشه
            var result = await _chatMessageServiceContract.SendGroupMessageAsync(senderId.Value,messageDto);

            // 2. فقط ارسال پیام به اعضای آنلاین
            foreach (var memberId in result.MemberIds.Where(m => m != senderId.Value))
            {
                var connections = await _userOnlineStatusServiceContract.GetUserConnectionsByIdAsync(memberId.ToString());

                if (connections?.Any() == true)
                {
                    var tasks = connections.Select(conn =>
                        Clients.Client(conn).SendAsync("ReceiveMessage", result));
                    await Task.WhenAll(tasks);
                }
            }

            // 3. بروزرسانی لیست همه اعضا
            var allMemberIds = result.MemberIds.Select(m => m.ToString()).ToList();
            await Clients.Users(allMemberIds).SendAsync("UpdateConversationList", result);

            // 4. تایید به فرستنده
            await Clients.Caller.SendAsync("MessageSent", result);
        }
        catch (ValidationException ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
        catch (Exception)
        {
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task MarkMessagesAsSeen(List<Guid> messageIds)
    {
        try
        {
            var userId = GetUserId();

            if (!userId.HasValue)
            {
                await Clients.Caller.SendAsync("Error", "User not authenticated");
                return;
            }

            await _chatMessageServiceContract.MarkMessagesAsSeenAsync(userId.Value, messageIds);

            await Clients.Caller.SendAsync("MessagesMarkedAsSeen", messageIds);
        }
        catch (Exception)
        {
            await Clients.Caller.SendAsync("Error", "Failed to mark messages as seen");
        }
    }

    public async Task LoadConversation(Guid conversationId)
    {
        var userId = GetUserId();
    }

    private Guid? GetUserId()
    {
        var userIdString = Context.UserIdentifier;

        if (string.IsNullOrWhiteSpace(userIdString))
            return null;

        return Guid.TryParse(userIdString, out var userId) ? userId : null;
    }
}