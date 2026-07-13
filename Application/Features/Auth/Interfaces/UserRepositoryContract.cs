using Application.Features.Auth.DTOs;
using Domain.Entities;

namespace Application.Features.Auth.Interfaces;

public interface UserRepositoryContract
{
    Task RegisterUserAsync(Domain.Entities.User user);
    Task<List<Domain.Entities.User>?> GetUsersByEmailOrUserNameAsync(string identifier);
    Task<bool> IsUserExistsByIdAsync(Guid userId);
    Task<bool> IsUserExistsByEmailAsync(string email);
    Task<bool> IsUserExistsByUserNameAsync(string userName);
    Task SaveChangesAsync();
    Task<List<ViewUser>> GetAllUsersAsync();
    Task<Domain.Entities.User?> GetUserByIdAsync(Guid userId);
    Task<Dictionary<Guid,string>> GetUserNamesByIdAsync(List<Guid> userIds);
    Task<List<Domain.Entities.User>> GetUsersByIdsAsync(List<Guid> userIds);
}