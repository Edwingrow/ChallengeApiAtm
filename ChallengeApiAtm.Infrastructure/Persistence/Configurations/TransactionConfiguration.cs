using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChallengeApiAtm.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    /// <summary>
    /// Configura el mapeo de la entidad Transaction
    /// </summary>
    /// <param name="builder">Constructor de entidad</param>
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
       
        builder.ToTable("transactions");

       
        builder.HasKey(t => t.Id);

       
        builder.Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(t => t.AccountId)
            .IsRequired();

        builder.Property(t => t.CardId)
            .IsRequired(false); 

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(TransactionStatus.Completed);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.BalanceAfterTransaction)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(t => t.AccountId)
            .HasDatabaseName("IX_Transactions_AccountId");

        builder.HasIndex(t => t.CardId)
            .HasDatabaseName("IX_Transactions_CardId");

        builder.HasIndex(t => t.Type)
            .HasDatabaseName("IX_Transactions_Type");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Transactions_CreatedAt");

        builder.HasIndex(t => new { t.AccountId, t.CreatedAt })
            .HasDatabaseName("IX_Transactions_AccountId_CreatedAt");

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Card)
            .WithMany()
            .HasForeignKey(t => t.CardId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
} 