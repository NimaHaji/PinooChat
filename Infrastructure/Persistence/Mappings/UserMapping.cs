using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class UserMapping:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(x => x.UserName)
            .IsUnique();
            
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.MobilePhone)
            .HasMaxLength(11);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.PasswordResetCodeHash)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.PasswordResetCodeExpireAt);
        
        builder.Property(x => x.PasswordResetAttemptsCount)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.HasMany(us=>us.Sessions)
            .WithOne(u=>u.User)
            .HasForeignKey(u=>u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(us => us.UserRole)
            .IsRequired()
            .HasDefaultValue(UserRole.User);
        
        builder.HasCheckConstraint(
            "CK_Role_Valid_Values",
            "[UserRole] IN (0, 1)"
        );
    }
}