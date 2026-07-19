using System.Reflection;
using Domain.Entities;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Contexts; 

public class ChatContext:DbContext
{
    public ChatContext(DbContextOptions options) : base(options)
    {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<Group> Groups { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly=Assembly.GetAssembly(typeof(UserMapping));
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        base.OnModelCreating(modelBuilder);
    }
}