using ChallengeApiAtm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeApiAtm.Infrastructure.Persistence.Configurations;


public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configura el mapeo de la entidad User
    /// </summary>
    /// <param name="builder">Constructor de entidad</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
      
        builder.ToTable("users");

       
        builder.HasKey(u => u.Id);

        
        builder.Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedNever(); 

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.DocumentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

       
        builder.Ignore(u => u.FullName);

       
        builder.HasIndex(u => u.DocumentNumber)
            .IsUnique()
            .HasDatabaseName("IX_Users_DocumentNumber");

        
        builder.HasMany(u => u.Cards)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Accounts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 