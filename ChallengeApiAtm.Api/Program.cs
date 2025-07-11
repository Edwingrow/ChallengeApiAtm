using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Application.Services;
using ChallengeApiAtm.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Challenge ATM API", 
        Version = "v1",
        Description = "API para sistema ATM con funcionalidades de login, consulta de saldo, retiros y operaciones"
    });
    
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = @"JWT Authorization header usando Bearer scheme. 
                      Ingresa 'Bearer' [espacio] y luego tu token en el campo de abajo.
                      Ejemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("La clave secreta no está configurada");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "ChallengeApiAtm",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "ChallengeApiAtm-Users",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<ChallengeApiAtm.Application.Validators.LoginRequestValidator>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAtmService, AtmService>();
builder.Services.AddScoped<INumberGeneratorService, NumberGeneratorService>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

/*
* Swagger
*/

/*
* Muestra el Swagger tanto en desarrollo como en producción.
*/

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Challenge ATM API v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseMiddleware<ChallengeApiAtm.Api.Middleware.ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    await app.Services.InitializeDatabaseAsync();
    Console.WriteLine("✅ Base de datos inicializada correctamente");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error inicializando base de datos: {ex.Message}");
}

app.Run();
public partial class Program { }