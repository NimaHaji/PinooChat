using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class UserSessionMapping:IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired();
        
        builder.Property(x => x.TokenHash)
            .IsRequired();
        
        builder.Property(x => x.AccessTokenHash)
            .IsRequired();
        
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
        
        builder.Property(x => x.ExpiredAtUtc)
            .IsRequired();
        
        builder.Property(x => x.RevokedAtUtc);
        
        builder.Property(x => x.LastSeenAtUtc)
            .IsRequired();

        builder.Property(x => x.UserAgent);
        builder.Property(x => x.DeviceName);
        builder.Property(x => x.IpAddress);
        builder.Property(x => x.ReplacedBySessionId);
        builder.Property(x => x.RevokedReason);
    }
}