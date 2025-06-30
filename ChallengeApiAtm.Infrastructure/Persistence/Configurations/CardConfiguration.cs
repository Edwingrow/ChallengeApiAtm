using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeApiAtm.Infrastructure.Persistence.Configurations;


public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    /// <summary>
    /// Configura el mapeo de la entidad Card
    /// </summary>
    /// <param name="builder">Constructor de entidad</param>
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        
        builder.ToTable("cards");

       
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedNever(); 

        builder.Property(c => c.CardNumber)
            .IsRequired()
            .HasMaxLength(19);

        builder.Property(c => c.HashedPin)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.AccountId)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(CardStatus.Active);

        builder.Property(c => c.FailedAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.ExpiryDate)
            .IsRequired();

        builder.Ignore(c => c.IsActive);

        builder.HasIndex(c => c.CardNumber)
            .IsUnique()
            .HasDatabaseName("IX_Cards_CardNumber");

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_Cards_UserId");

        builder.HasIndex(c => c.AccountId)
            .HasDatabaseName("IX_Cards_AccountId");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Cards_Status");

        
        builder.HasOne(c => c.User)
            .WithMany(u => u.Cards)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Account)
            .WithMany()
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 