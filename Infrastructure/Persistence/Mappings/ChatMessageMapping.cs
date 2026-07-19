using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class ChatMessageMapping:IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x=>x.ConversationId)
            .IsRequired();

        builder
            .Property(x => x.SenderId)
            .IsRequired();
        
        builder
            .Property(x => x.ReceiverId);
        
        builder
            .Property(x => x.Content)
            .IsRequired();
        
        builder
            .Property(x => x.TimeStamp)
            .IsRequired();
        
        builder
            .Property(x=>x.MessageStatus)
            .IsRequired();
        
        builder
            .Property(x=>x.SeenAt);
        
        builder
            .Property(x=>x.DeliveredAt)
            .IsRequired();

        builder
            .HasOne(x => x.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(x => x.Sender)
            .WithMany(u => u.ChatMessages)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder
            .HasOne(x => x.ReplyTo)
            .WithMany(cm => cm.Replies)
            .HasForeignKey(x => x.ReplyToId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasIndex(x => x.ConversationId);
        builder.HasIndex(x => x.SenderId);
        builder.HasIndex(x => x.ReplyToId);
    }
}