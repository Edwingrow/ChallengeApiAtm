using ChallengeApiAtm.Application.DTOs.Requests;
using FluentValidation;
using System.Globalization;

namespace ChallengeApiAtm.Application.Validators;

/// <summary>
/// Validador para RegisterRequest
/// </summary>
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private const decimal MinInitialBalance = 1000000;

    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio")
            .MinimumLength(2)
            .WithMessage("El nombre debe tener al menos 2 caracteres")
            .MaximumLength(50)
            .WithMessage("El nombre no puede tener más de 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El nombre solo puede contener letras y espacios");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es obligatorio")
            .MinimumLength(2)
            .WithMessage("El apellido debe tener al menos 2 caracteres")
            .MaximumLength(50)
            .WithMessage("El apellido no puede tener más de 50 caracteres")
            .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$")
            .WithMessage("El apellido solo puede contener letras y espacios");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .WithMessage("El número de documento es obligatorio")
            .MinimumLength(8)
            .WithMessage("El documento debe tener al menos 8 caracteres")
            .MaximumLength(20)
            .WithMessage("El documento no puede tener más de 20 caracteres")
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("El documento solo puede contener letras y números");

        RuleFor(x => x.Pin)
            .NotEmpty()
            .WithMessage("El PIN es obligatorio")
            .Length(3, 4)
            .WithMessage("El PIN debe tener 3 o 4 dígitos")
            .Matches(@"^\d{3,4}$")
            .WithMessage("El PIN solo puede contener dígitos");

        RuleFor(x => x.ConfirmPin)
            .NotEmpty()
            .WithMessage("La confirmación del PIN es obligatoria")
            .Equal(x => x.Pin)
            .WithMessage("La confirmación del PIN no coincide");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(MinInitialBalance)
            .WithMessage($"El saldo inicial no puede ser menor a ${MinInitialBalance}");

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("El número de tarjeta es obligatorio")
            .MinimumLength(16)
            .WithMessage("El número de tarjeta debe tener al menos 16 dígitos")
            .MaximumLength(19)
            .WithMessage("El número de tarjeta no puede tener más de 19 dígitos")
            .Matches(@"^\d+$")
            .WithMessage("El número de tarjeta solo puede contener dígitos");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty()
            .WithMessage("La fecha de vencimiento es obligatoria")
            .Matches(@"^(0[1-9]|1[0-2])\/\d{4}$")
            .WithMessage("La fecha de vencimiento debe tener el formato MM/yyyy")
            .Must(BeValidExpiryDate)
            .WithMessage("La fecha de vencimiento debe ser mayor a la fecha actual");
    }

    /// <summary>
    /// Valida que la fecha de vencimiento sea válida y mayor a la fecha actual
    /// </summary>
    /// <param name="expiryDate">Fecha en formato MM/yyyy</param>
    /// <returns>True si es válida</returns>
    private static bool BeValidExpiryDate(string expiryDate)
    {
        if (string.IsNullOrEmpty(expiryDate))
            return false;

        if (!DateTime.TryParseExact($"01/{expiryDate}", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            return false;

        // La tarjeta vence al final del mes especificado
        var expiryEndOfMonth = parsedDate.AddMonths(1).AddDays(-1);
        
        // Usar UTC para consistencia con la base de datos
        return expiryEndOfMonth > DateTime.UtcNow;
    }
}