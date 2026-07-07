using Application.Features.Auth.DTOs;
using Application.Features.Auth.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : UserRepositoryContract
{
    private readonly ChatContext _context;

    public UserRepository(ChatContext context)
    {
        _context = context;
    }

    public async Task RegisterUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await SaveChangesAsync();
    }

    public async Task<List<User>?> GetUsersByEmailOrUserNameAsync(string identifier)
    {
        return await _context
            .Users
            .Where(x => identifier == x.Email || x.UserName == identifier)
            .ToListAsync();
    }

    public async Task<bool> IsUserExistsByIdAsync(Guid userId)
    {
        return await _context.Users.AnyAsync(x => x.Id == userId);
    }

    public async Task<bool> IsUserExistsByEmailAsync(string email)
    {
        return await _context
            .Users
            .AnyAsync(x => x.Email == email);
    }

    public async Task<bool> IsUserExistsByUserNameAsync(string userName)
    {
        return await _context
            .Users
            .AnyAsync(x => x.UserName == userName);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<ViewUser>> GetAllUsersAsync()
    {
        return await _context
            .Users
            .Select(x => new ViewUser
            {
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
                PhoneNumber = x.MobilePhone,
            }).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context
            .Users
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();
    }
}