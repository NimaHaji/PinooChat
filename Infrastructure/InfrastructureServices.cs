using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Auth.Interfaces;
using Application.Features.Session.Interfaces;
using Domain;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security.Hashing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatContext>(options =>options.UseSqlServer(configuration.GetConnectionString("connect")));
        services.AddScoped<UnitOfWorkContract,UnitOfWork>();
        services.AddScoped<UserRepositoryContract,UserRepository>();
        services.AddScoped<UserContextContract,UserContext>();
        services.AddScoped<UserSessionRepositoryContract,UserSessionRepository>();
        services.AddScoped<IHasher, Sha256Hasher>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }
}