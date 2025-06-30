using ChallengeApiAtm.Application.DTOs.Requests;
using FluentValidation;

namespace ChallengeApiAtm.Application.Validators;

/// <summary>
/// Validador para las solicitudes de login en el ATM
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// Inicializa el validador con las reglas de negocio
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("El número de tarjeta es obligatorio")
            .MinimumLength(16)
            .WithMessage("El número de tarjeta debe tener al menos 16 dígitos")
            .MaximumLength(19)
            .WithMessage("El número de tarjeta no puede tener más de 19 dígitos")
            .Matches(@"^\d+$")
            .WithMessage("El número de tarjeta solo puede contener dígitos");

        RuleFor(x => x.Pin)
            .NotEmpty()
            .WithMessage("El PIN es obligatorio")
            .Length(3, 4)
            .WithMessage("El PIN debe tener 3 o 4 dígitos")
            .Matches(@"^\d{3,4}$")
            .WithMessage("El PIN solo puede contener dígitos");
    }
} 