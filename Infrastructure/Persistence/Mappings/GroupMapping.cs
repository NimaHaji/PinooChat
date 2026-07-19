using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class GroupMapping:IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        // public Guid ConversationId { get; private set; }
        //
        // public string GroupIdName { get; private set; }
        //
        // public string GroupTitle { get; private set; }
        //
        // public string? Description { get; private set; }
        //
        // public Guid OwnerId { get; private set; }
        //
        // public Conversation Conversation { get; private set; } = null!;
        builder.ToTable("Groups");
        
        builder.HasKey(g => g.ConversationId);

        builder.Property(g=>g.GroupIdName)
            .IsRequired()
            .HasMaxLength(30);
        
        builder.HasIndex(x => x.GroupIdName)
            .IsUnique();
        
        builder.Property(g=>g.GroupTitle)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.OwnerId)
            .IsRequired();
        
        builder.Property(g=>g.Description)
            .HasMaxLength(500);

        builder
            .HasOne(g => g.Conversation)
            .WithOne(c => c.Group)
            .HasForeignKey<Group>(g => g.ConversationId);
    }
}