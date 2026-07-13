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
        
        builder.Property(x => x.LastMessage).IsRequired();
        
        builder.Property(x => x.LastMessageAt).IsRequired();
        
        builder
            .HasMany(x => x.Participants)
            .WithOne(c=>c.Conversation)
            .HasForeignKey(f=>f.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey(f=>f.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
    }
}