using Application.Features.Auth.Interfaces;
using Application.Features.User.DTOs;
using Application.Features.User.Interface;
using Shared.Exceptions;

namespace Application.Features.User.Implement;

public class UserOnlineStatusService : UserOnlineStatusServiceContract
{
    private readonly UserOnlineStatusRepositoryContract _onlineStatusRepositoryContract;
    private readonly UserRepositoryContract _userRepositoryContract;

    public UserOnlineStatusService(UserOnlineStatusRepositoryContract onlineStatusRepositoryContract,
        UserRepositoryContract userRepositoryContract)
    {
        _onlineStatusRepositoryContract = onlineStatusRepositoryContract;
        _userRepositoryContract = userRepositoryContract;
    }

    public async Task<List<string>> GetOnlineUsersAsync()
    {
        var onlineUsersId = await _onlineStatusRepositoryContract.GetOnlineUsersAsync();

        var UsersIdGuid = onlineUsersId
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => Guid.Parse(id))
            .ToList();

        var userDict = await _userRepositoryContract.GetUserNamesByIdAsync(UsersIdGuid);

        var onlineUserNames = UsersIdGuid
            .Where(id => userDict.ContainsKey(id))
            .Select(id => userDict[id])
            .ToList();

        return onlineUserNames;
    }

    public async Task MarkUserOnlineAsync(string userId, string connectionId)
    {
        await _onlineStatusRepositoryContract.AddUserConnectionAsync(userId, connectionId);
    }

    public async Task<UserOfflineDto> MarkUserOfflineAsync(string connectionId)
    {
        var userIdValue = await _onlineStatusRepositoryContract.GetUserIdByConnectionIdAsync(connectionId);

        if (!string.IsNullOrWhiteSpace(userIdValue))
            throw new NotFoundException("کاربر یافت نشد");

        await _onlineStatusRepositoryContract.RemoveConnectionAsync(connectionId, userIdValue);
        await _onlineStatusRepositoryContract.RemoveConnectionMappingAsync(connectionId);

        var remainingConnections = await _onlineStatusRepositoryContract.GetConnectionCountAsync(connectionId);

        if (remainingConnections == 0)
        {
            await _onlineStatusRepositoryContract.RemoveUserFormOnlineListAsync(userIdValue);
            await _onlineStatusRepositoryContract.DeleteUserConnectionsAsync(userIdValue);

            return new UserOfflineDto
            {
                IsUserCompletelyOffline = true,
                UserId = userIdValue,
                RemainingConnections = 0
            };
        }

        return new UserOfflineDto
        {
            IsUserCompletelyOffline = false,
            UserId = userIdValue,
            RemainingConnections = remainingConnections
        };
    }

    public async Task<List<string>> GetUserConnectionsByIdAsync(string receiverId)
    {
        return await _onlineStatusRepositoryContract.GetUserConnectionsAsync(receiverId);
    }
}