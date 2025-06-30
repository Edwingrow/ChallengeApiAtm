using ChallengeApiAtm.Application.DTOs.Requests;
using FluentValidation;

namespace ChallengeApiAtm.Application.Validators;

/// <summary>
/// Validador para las solicitudes de consulta de saldo
/// </summary>
public sealed class BalanceRequestValidator : AbstractValidator<BalanceRequest>
{
    public BalanceRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("El número de tarjeta es obligatorio")
            .Length(16, 19)
            .WithMessage("El número de tarjeta debe tener entre 16 y 19 dígitos")
            .Matches(@"^\d+$")
            .WithMessage("El número de tarjeta debe contener solo números");
    }
} 