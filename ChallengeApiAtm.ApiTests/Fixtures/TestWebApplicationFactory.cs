// ChallengeApiAtm.ApiTests/Fixtures/TestWebApplicationFactory.cs
using ChallengeApiAtm.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChallengeApiAtm.ApiTests.Fixtures;

/// <summary>
/// Factory para configurar TestServer para API tests
/// </summary>
public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> 
    where TProgram : class
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // ✅ CORRECCIÓN: Remover DbContext correctamente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AtmDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // ✅ CORRECCIÓN: Usar nombre de BD fijo para todos los tests
            services.AddDbContext<AtmDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
                options.EnableSensitiveDataLogging();
            });
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Método para seedear datos de prueba
    /// </summary>
    public async Task SeedTestDataAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtmDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<ChallengeApiAtm.Application.Interfaces.IPasswordHasher>();
        
        try
        {
            await context.Database.EnsureCreatedAsync();
            
            // ✅ CORRECCIÓN: Solo limpiar si hay datos
            if (context.Users.Any())
            {
                context.Users.RemoveRange(context.Users);
                context.Accounts.RemoveRange(context.Accounts);
                context.Cards.RemoveRange(context.Cards);
                await context.SaveChangesAsync();
            }

            // ✅ CORRECCIÓN: Verificar que no existan datos antes de crear
            if (!context.Users.Any(u => u.DocumentNumber == "96069288"))
            {
                // Crear datos de prueba
                var user = new ChallengeApiAtm.Domain.Entities.User("Edwin", "Garcia", "96069288");
                context.Users.Add(user);
                await context.SaveChangesAsync();

                var account = new ChallengeApiAtm.Domain.Entities.Account("1420284881", user.Id, 1000000.00m);
                context.Accounts.Add(account);
                await context.SaveChangesAsync();

                var card = new ChallengeApiAtm.Domain.Entities.Card("4001234567890101", passwordHasher.HashPin("8033"), user.Id, account.Id);
                context.Cards.Add(card);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // ✅ CORRECCIÓN: Logging de errores
            Console.WriteLine($"Error en SeedTestDataAsync: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Limpia la base de datos
    /// </summary>
    public async Task CleanupAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtmDbContext>();
        
        try
        {
            await context.Database.EnsureDeletedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en CleanupAsync: {ex.Message}");
        }
    }
}