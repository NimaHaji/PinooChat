using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Application.Features.User.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hub;

[Authorize]
public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly ChatMessageServiceContract _chatMessageServiceContract;
    private readonly UserOnlineStatusServiceContract _userOnlineStatusServiceContract;

    public ChatHub(ChatMessageServiceContract chatMessageServiceContract, UserOnlineStatusServiceContract userOnlineStatusService)
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
        await base.OnDisconnectedAsync(exception);
        
        var connectionId = Context.ConnectionId;
        var result = await _userOnlineStatusServiceContract.MarkUserOfflineAsync(connectionId);

        if (result.IsUserCompletelyOffline)
        {
            await Clients.Others.SendAsync("UserOffline", result.UserId);
        }
    }

    public async Task GetOnlineUsers()
    {
        var onlineUsers = await _userOnlineStatusServiceContract.GetOnlineUsersAsync();
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
    }

    public async Task SendPrivateMessage(string receiverIdString, string content)
    {
        var senderId= GetUserId();
        if (!senderId.HasValue)
        {
            await Clients.Caller.SendAsync("Error", "User not authenticated");
            return;
        }

        if (!Guid.TryParse(receiverIdString, out var receiverId))
        {
            await Clients.Caller.SendAsync("Error", "Invalid receiver identifier");
            return;
        }

        if (senderId.Value == receiverId)
        {
            await Clients.Caller.SendAsync("Error", "Cannot send message to yourself");
            return;
        }
        var dto = new SendMessageDto
        {
            SenderId = senderId.Value,
            ReceiverId = receiverId,
            Content = content
        };
        var result = await _chatMessageServiceContract.SendMessageAsync(dto);
        
        var receiverConnections=await _userOnlineStatusServiceContract.GetUserConnectionsByIdAsync(receiverId.ToString());
        var isReceiverOnline = receiverConnections.Any();

        if (isReceiverOnline)
        {
            var tasks = receiverConnections.Select(connectionId =>
                Clients.Client(connectionId).SendAsync("ReceiveMessage", result));

            await Task.WhenAll(tasks);
            
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
            await Clients.Caller.SendAsync("UserOffline", new
            {
                ReceiverId = receiverId.ToString(),
                Message = result,
                WillBeDeliveredLater = true
            });
        }
        
        await Clients.Caller.SendAsync("MessageSent", result);
    }
    public async Task MarkMessagesAsSeen(List<Guid> messageIds)
    {
        try
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            await _chatMessageServiceContract.MarkMessagesAsSeenAsync(userId.Value, messageIds);
            
            await Clients.Caller.SendAsync("MessagesMarkedAsSeen", messageIds);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", "Failed to mark messages as Seen");
        }
    }
    private Guid? GetUserId()
    {
        var userIdString = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userIdString))
            return null;

        return Guid.TryParse(userIdString, out var userId) ? userId : null;

    }
}