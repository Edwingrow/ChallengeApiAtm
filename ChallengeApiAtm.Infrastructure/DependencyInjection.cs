using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.Infrastructure.Persistence.Context;
using ChallengeApiAtm.Infrastructure.Repositories;
using ChallengeApiAtm.Infrastructure.Security;
using ChallengeApiAtm.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace ChallengeApiAtm.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra los servicios de Infrastructure en el contenedor de dependencias
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios configurada</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AtmDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AtmDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            })
            .UseSnakeCaseNamingConvention(); // ← Configurar snake_case naming

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        
        // Servicios de contexto HTTP
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtmDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        try
        {
            Console.WriteLine("🔄 Verificando y aplicando migraciones de Entity Framework...");
            
            await context.Database.MigrateAsync();
            Console.WriteLine("✅ Migraciones aplicadas correctamente");

            // Solo cargar datos de prueba en Development
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            
            if (isDevelopment)
            {
                // Verificar si necesitamos datos de prueba (solo si no hay usuarios)
                var userExists = await context.Users.AnyAsync();
                if (!userExists)
                {
                    Console.WriteLine("📊 [DEVELOPMENT] Creando datos de prueba...");
                    await SeedTestDataAsync(context, passwordHasher);
                    Console.WriteLine("✅ [DEVELOPMENT] Datos de prueba creados correctamente");
                }
                else
                {
                    Console.WriteLine("📊 [DEVELOPMENT] Datos de prueba ya existen, omitiendo creación");
                }
            }
            else
            {
                Console.WriteLine("🏭 [PRODUCTION] Datos de prueba omitidos (solo en Development)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inicializando la base de datos: {ex.Message}");
            throw;
        }
    }

    private static async Task SeedTestDataAsync(AtmDbContext context, IPasswordHasher passwordHasher)
    {
        var users = new[]
        {
            new Domain.Entities.User("Edwin", "Garcia", "96069288"),
            new Domain.Entities.User("Ana", "Martinez", "87654321"), 
            new Domain.Entities.User("Carlos", "Rodriguez", "11223344"),
            new Domain.Entities.User("Maria", "Lopez", "99887766")
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var accounts = new[]
        {
            new Domain.Entities.Account("1420284881", users[0].Id, 1000000.00m),
            new Domain.Entities.Account("1420284882", users[1].Id, 500000.00m),  
            new Domain.Entities.Account("1420284883", users[2].Id, 750000.00m),  
            new Domain.Entities.Account("1420284884", users[3].Id, 250000.00m)
        };

        await context.Accounts.AddRangeAsync(accounts);
        await context.SaveChangesAsync();

        var cards = new[]
        {
            new Domain.Entities.Card("4001234567890101", passwordHasher.HashPin("803"), users[0].Id, accounts[0].Id),
            new Domain.Entities.Card("4001234567890102", passwordHasher.HashPin("123"), users[1].Id, accounts[1].Id),
            new Domain.Entities.Card("4001234567890103", passwordHasher.HashPin("567"), users[2].Id, accounts[2].Id),
            new Domain.Entities.Card("4001234567890104", passwordHasher.HashPin("2075"), users[3].Id, accounts[3].Id)
        };

        await context.Cards.AddRangeAsync(cards);
        await context.SaveChangesAsync();

        var transactions = new List<Domain.Entities.Transaction>();
        var random = new Random(12345);
        var startDate = DateTime.UtcNow.AddDays(-30);

        var balanceInquiries = 0;
        var withdrawals = 0;

        for (int i = 0; i < 60; i++)
        {
            var accountIndex = random.Next(0, accounts.Length);
            var account = accounts[accountIndex];
            var card = cards[accountIndex];
            var user = users[accountIndex];

            var transactionDate = startDate.AddDays(random.Next(0, 30))
                                           .AddHours(random.Next(8, 20))
                                           .AddMinutes(random.Next(0, 60));

            var transactionTypeRoll = random.Next(1, 101);
            Domain.Enums.TransactionType transactionType;
            decimal amount;
            string description;
            var status = Domain.Enums.TransactionStatus.Completed;

            if (transactionTypeRoll <= 60) // 60% Consultas de saldo
            {
                transactionType = Domain.Enums.TransactionType.BalanceInquiry;
                amount = 0.00m;
                description = "Consulta de saldo";
                balanceInquiries++;
            }
            else // 40% Retiros
            {
                transactionType = Domain.Enums.TransactionType.Withdrawal;
                var withdrawAmounts = new[] { 50000m, 100000m, 200000m, 500000m, 1000000m };
                amount = withdrawAmounts[random.Next(withdrawAmounts.Length)];
                
                // 5% de probabilidad de retiro fallido por fondos insuficientes
                if (random.Next(1, 101) <= 5)
                {
                    status = Domain.Enums.TransactionStatus.Failed;
                    description = "Retiro ATM - Rechazado: Fondos insuficientes";
                }
                else
                {
                    description = "Retiro ATM - Confirmado";
                }
                withdrawals++;
            }

            // Calcular balance después de transacción (simulado)
            decimal? balanceAfterTransaction = null;
            if (transactionType != Domain.Enums.TransactionType.BalanceInquiry && status == Domain.Enums.TransactionStatus.Completed)
            {
                var currentBalance = account.Balance;
                balanceAfterTransaction = transactionType switch
                {
                    Domain.Enums.TransactionType.Withdrawal => Math.Max(0, currentBalance - amount),
                    _ => currentBalance
                };
            }

            var transaction = new Domain.Entities.Transaction(
                accountId: account.Id,
                cardId: card.Id,
                type: transactionType,
                amount: amount,
                description: description
            );

            // Configurar propiedades adicionales usando reflection (ya que el constructor es básico)
            typeof(Domain.Entities.Transaction).GetProperty("Status")?.SetValue(transaction, status);
            typeof(Domain.Entities.Transaction).GetProperty("CreatedAt")?.SetValue(transaction, transactionDate);
            typeof(Domain.Entities.Transaction).GetProperty("BalanceAfterTransaction")?.SetValue(transaction, balanceAfterTransaction);

            transactions.Add(transaction);
        }

        // Ordenar por fecha para mayor realismo
        transactions = transactions.OrderBy(t => t.CreatedAt).ToList();

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();

        // 📊 ESTADÍSTICAS DE DATOS CREADOS
        Console.WriteLine("📋 Datos de prueba creados:");
        Console.WriteLine($"   👥 Usuarios: {users.Length}");
        Console.WriteLine($"   🏦 Cuentas: {accounts.Length}");
        Console.WriteLine($"   💳 Tarjetas: {cards.Length}");
        Console.WriteLine($"   💸 Transacciones: {transactions.Count}");
        Console.WriteLine();
        Console.WriteLine("📈 Distribución de transacciones:");
        Console.WriteLine($"   🔍 Consultas de saldo: {balanceInquiries}");
        Console.WriteLine($"   💰 Retiros: {withdrawals}");
        Console.WriteLine();
        Console.WriteLine("👤 Usuarios de prueba:");
        for (int i = 0; i < users.Length; i++)
        {
            Console.WriteLine($"   {users[i].FirstName} {users[i].LastName}:");
            Console.WriteLine($"     📄 Documento: {users[i].DocumentNumber}");
            Console.WriteLine($"     💳 Tarjeta: {cards[i].CardNumber}");
            Console.WriteLine($"     🔢 PIN: {(i == 0 ? "8033" : i == 1 ? "1234" : i == 2 ? "5678" : "9999")}");
            Console.WriteLine($"     🏦 Cuenta: {accounts[i].AccountNumber}");
            Console.WriteLine($"     💰 Saldo: ${accounts[i].Balance:N2}");
            Console.WriteLine();
        }

        Console.WriteLine("🎯 Usuario principal para pruebas:");
        Console.WriteLine($"   Tarjeta: {cards[0].CardNumber}");
        Console.WriteLine($"   PIN: 8033");
        Console.WriteLine($"   Transacciones disponibles: {transactions.Count(t => t.AccountId == accounts[0].Id)}");
    }

    public static async Task<bool> IsDatabaseAvailableAsync(this IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AtmDbContext>();
            return await context.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }
} 