using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class ConversationMapping:IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        
        builder.HasKey(x => x.Id);

        builder
            .Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.LastMessage);
        
        builder.Property(x => x.LastMessageAt);

        builder
            .Property(x => x.ConversationType)
            .IsRequired();
        
        builder
            .HasMany(x => x.Participants)
            .WithOne(c=>c.Conversation)
            .HasForeignKey(f=>f.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(c => c.Messages)
            .WithOne(x=>x.Conversation)
            .HasForeignKey(f=>f.ConversationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}