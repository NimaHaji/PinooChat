using Application.Features.User.DTOs;

namespace Application.Features.User.Interface;

public interface UserOnlineStatusServiceContract
{
     Task<List<string>> GetOnlineUsersAsync();
     Task MarkUserOnlineAsync(string userId, string connectionId);
     Task<UserOfflineDto> MarkUserOfflineAsync(string connectionId);
     Task<List<string>> GetUserConnectionsByIdAsync(string receiverId);
}