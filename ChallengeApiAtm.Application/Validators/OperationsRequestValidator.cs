using ChallengeApiAtm.Application.DTOs.Requests;
using FluentValidation;

namespace ChallengeApiAtm.Application.Validators;


public sealed class OperationsRequestValidator : AbstractValidator<OperationsRequest>
{
    private const int MaxPageSize = 10;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;

    /// <summary>
    /// Inicializa el validador con las reglas de negocio
    /// </summary>
    public OperationsRequestValidator()
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
    }
} 