using Application.Features.ChatMessages.DTOs;
using Application.Features.ChatMessages.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace Api.Hub;

[Authorize]
public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly ChatMessageServiceContract _chatMessageServiceContract;
    private readonly IDatabase _redisDatabase;

    public ChatHub(ChatMessageServiceContract chatMessageServiceContract, IDatabase redisDatabase)
    {
        _chatMessageServiceContract = chatMessageServiceContract;
        _redisDatabase = redisDatabase;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await _redisDatabase.HashSetAsync("online_users", userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await _redisDatabase.HashDeleteAsync("online_users", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendPrivateMessage(string receiverIdString, string content)
    {
        var senderIdString = Context.UserIdentifier;
        if (string.IsNullOrEmpty(senderIdString)) return;

        if (!Guid.TryParse(senderIdString, out var senderId))
        {
            await Clients.Caller.SendAsync("Error", "User identifier is not a valid GUID");
            return;
        }

        if (!Guid.TryParse(receiverIdString, out var receiverId))
        {
            await Clients.Caller.SendAsync("Error", "Receiver ID is not a valid GUID");
            return;
        }

        var dto = new SendMessageDto
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content
        };
        var result = await _chatMessageServiceContract.SendMessageAsync(dto);

        var receiverConnectionId = await _redisDatabase.HashGetAsync("online_users", receiverId.ToString());

        if (!receiverConnectionId.IsNullOrEmpty)
        {
            await Clients.Client(receiverConnectionId.ToString())
                .SendAsync("ReceiveMessage", result);
        }

        await Clients.Caller.SendAsync("MessageSent", result);
    }
}