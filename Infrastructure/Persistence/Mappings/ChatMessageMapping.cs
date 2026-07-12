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

        builder
            .Property(x => x.SenderId)
            .IsRequired();
        
        builder
            .Property(x => x.ReceiverId)
            .IsRequired();
        
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
            .Property(x=>x.SeenAt)
            .IsRequired();
        
        builder
            .Property(x=>x.DeliveredAt)
            .IsRequired();
    }
}