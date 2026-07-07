using System.Reflection;
using Domain.Entities;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts; 

public class ChatContext:DbContext
{
    public ChatContext(DbContextOptions options) : base(options)
    {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly=Assembly.GetAssembly(typeof(UserMapping));
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        base.OnModelCreating(modelBuilder);
    }
}