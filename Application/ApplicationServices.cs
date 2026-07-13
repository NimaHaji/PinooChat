using System.ComponentModel.Design;
using Application.Features.Auth.Interfaces;
using Application.Features.Auth.Services;
using Application.Features.ChatMessages.Implement;
using Application.Features.ChatMessages.Repositories;
using Application.Features.Conversation.Implement;
using Application.Features.Conversation.Interfaces;
using Application.Features.Session.Interfaces;
using Application.Features.Session.Services;
using Application.Features.User.Implement;
using Application.Features.User.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UserServiceContract, UserService>();
        services.AddScoped<UserSessionServiceContract, UserSessionService>();
        services.AddScoped<ChatMessageServiceContract, ChatMessageService>();
        services.AddScoped<UserOnlineStatusServiceContract, UserOnlineStatusService>();
        services.AddScoped<ConversationServiceContract, ConversationService>();
        return services;
    }
}