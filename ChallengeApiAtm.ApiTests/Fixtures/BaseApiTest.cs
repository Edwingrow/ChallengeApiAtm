// ChallengeApiAtm.ApiTests/Fixtures/BaseApiTest.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit; // ✅ AGREGAR: Para IClassFixture<>
using Xunit.Sdk; // ✅ AGREGAR: Para IAsyncLifetime

namespace ChallengeApiAtm.ApiTests.Fixtures;

/// <summary>
/// Clase base para tests de API con configuración común
/// </summary>
public abstract class BaseApiTest : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly TestWebApplicationFactory<Program> Factory;
    protected readonly JsonSerializerOptions JsonOptions;

    protected BaseApiTest(TestWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Inicialización asíncrona
    /// </summary>
    public async Task InitializeAsync()
    {
        await Factory.SeedTestDataAsync();
    }

    /// <summary>
    /// Limpieza asíncrona
    /// </summary>
    public async Task DisposeAsync()
    {
        Client?.Dispose();
        await Factory.CleanupAsync();
    }

    /// <summary>
    /// Datos de usuario de prueba predefinidos
    /// </summary>
    protected static class TestData
    {
        public const string CardNumber = "4001234567890101";
        public const string Pin = "8033";
        public const string DocumentNumber = "96069288";
        public const string AccountNumber = "1420284881";
        public const decimal InitialBalance = 1000000.00m;
    }

    /// <summary>
    /// Realiza login y retorna el token JWT
    /// </summary>
    protected async Task<string> LoginAndGetTokenAsync()
    {
        var loginRequest = new
        {
            cardNumber = TestData.CardNumber,
            pin = TestData.Pin
        };

        var response = await Client.PostAsync("/api/auth/login", 
            new StringContent(JsonSerializer.Serialize(loginRequest, JsonOptions), 
                Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Login falló: {response.StatusCode} - {errorContent}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        
        using var jsonDoc = JsonDocument.Parse(content);
        var token = jsonDoc.RootElement.GetProperty("data").GetProperty("token").GetString();
        
        return token ?? throw new InvalidOperationException("No se pudo obtener el token");
    }

    /// <summary>
    /// Configura autorización en el cliente HTTP
    /// </summary>
    protected void SetAuthorization(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Realiza un POST con JSON
    /// </summary>
    protected async Task<HttpResponseMessage> PostJsonAsync<T>(string requestUri, T data)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await Client.PostAsync(requestUri, content);
    }
}