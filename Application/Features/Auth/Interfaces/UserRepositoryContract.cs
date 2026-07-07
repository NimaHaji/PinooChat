using Application.Features.Auth.DTOs;
using Domain.Entities;

namespace Application.Features.Auth.Interfaces;

public interface UserRepositoryContract
{
    Task RegisterUserAsync(User user);
    Task<List<User>?> GetUsersByEmailOrUserNameAsync(string identifier);
    Task<bool> IsUserExistsByIdAsync(Guid userId);
    Task<bool> IsUserExistsByEmailAsync(string email);
    Task<bool> IsUserExistsByUserNameAsync(string userName);
    Task SaveChangesAsync();
    Task<List<ViewUser>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
}