namespace Application.Features.User.Interface;

public interface UserOnlineStatusRepositoryContract
{
    Task<string?> GetUserIdByConnectionIdAsync(string connectionId);
    Task RemoveConnectionAsync(string connectionId, string userId);
    Task RemoveConnectionMappingAsync(string connectionId);
    Task<long> GetConnectionCountAsync(string userId);
    Task RemoveUserFormOnlineListAsync(string userId);
    Task DeleteUserConnectionsAsync(string userId);
    Task AddUserConnectionAsync(string userId, string connectionId);
    Task<List<string>> GetUserConnectionsAsync(string userId);
    Task<List<string>> GetOnlineUsersAsync();
    Task<bool> IsUserOnlineAsync(Guid userId);
}