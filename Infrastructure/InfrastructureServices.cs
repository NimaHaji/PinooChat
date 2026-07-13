using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.ChatMessages.Repositories;
using Application.Features.Conversation.Interfaces;
using Application.Features.Session.Interfaces;
using Application.Features.User.Interface;
using Domain;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Chat;
using Infrastructure.Persistence.Repositories.User;
using Infrastructure.Security.Hashing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatContext>(options =>options.UseSqlServer(configuration.GetConnectionString("connect")));
        services.AddSingleton<IDatabase>(sp =>
            sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
        services.AddScoped<UnitOfWorkContract,UnitOfWork>();
        services.AddScoped<UserRepositoryContract,UserRepository>();
        services.AddScoped<UserContextContract,UserContext>();
        services.AddScoped<UserSessionRepositoryContract,UserSessionRepository>();
        services.AddScoped<IHasher, Sha256Hasher>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ChatMessagesRepositoryContract, ChatMessageRepository>();
        services.AddScoped<UserOnlineStatusRepositoryContract, UserOnlineStatusRepository>();
        services.AddScoped<ConversationRepositoryContract, ConversationRepository>();
        return services;
    }
}