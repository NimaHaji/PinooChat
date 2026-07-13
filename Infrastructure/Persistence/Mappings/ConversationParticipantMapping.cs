using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class ConversationParticipantMapping : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");

        builder.HasKey(cp => new { cp.ConversationId, cp.UserId });

        builder
            .Property(cp => cp.UserId)
            .IsRequired();
        
        builder.Property(cp => cp.JoinedAt)
            .IsRequired();
        
        builder
            .HasOne(cp=>cp.Conversation)
            .WithMany(c=>c.Participants)
            .HasForeignKey(f=>f.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cp => cp.UserId);
    }
}