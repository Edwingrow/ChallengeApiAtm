using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApiAtm.Infrastructure.Persistence.Context;


public sealed class AtmDbContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia del contexto de base de datos
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto</param>
    public AtmDbContext(DbContextOptions<AtmDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Card> Cards { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    /// <summary>
    /// Configura el modelo de datos y las relaciones
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new CardConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        modelBuilder.HasDefaultSchema("atm");

        ConfigureGlobalSettings(modelBuilder);
    }

    /// <summary>
    /// Configura ajustes globales para todas las entidades
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo</param>
    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entityType.SetTableName(tableName.ToLowerInvariant());
            }
        }

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(string)))
        {
            if (property.GetMaxLength() == null)
            {
                property.SetMaxLength(200);
            }
        }
    }
}