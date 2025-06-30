using ChallengeApiAtm.Application.DTOs.Requests;
using FluentValidation;

namespace ChallengeApiAtm.Application.Validators;

/// <summary>
/// Validador para las solicitudes de retiro en el ATM
/// </summary>
public sealed class WithdrawRequestValidator : AbstractValidator<WithdrawRequest>
{
    private const decimal MaxWithdrawAmount = 10000000;
    private const decimal MinWithdrawAmount = 1000;

    /// <summary>
    /// Inicializa el validador con las reglas de negocio
    /// </summary>
    public WithdrawRequestValidator()
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

        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("El monto es obligatorio")
            .GreaterThanOrEqualTo(MinWithdrawAmount)
            .WithMessage($"El monto mínimo de retiro es ${MinWithdrawAmount}")
            .LessThanOrEqualTo(MaxWithdrawAmount)
            .WithMessage($"El monto máximo de retiro es ${MaxWithdrawAmount}")
            .Must(BeValidMultiple)
            .WithMessage("El monto debe ser múltiplo de 10")
            .Must(x => x > 0)
            .WithMessage("El monto debe ser mayor a cero");
    }
    private static bool BeValidMultiple(decimal amount)
    {
        return amount % 10 == 0;
    }
} 