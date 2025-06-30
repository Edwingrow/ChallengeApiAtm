using ChallengeApiAtm.Application.DTOs.Requests;
using FluentValidation;

namespace ChallengeApiAtm.Application.Validators;

public sealed class UnblockCardRequestValidator : AbstractValidator<UnblockCardRequest>
{
    public UnblockCardRequestValidator()
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

        RuleFor(x => x.DocumentNumber)
            .NotEmpty()
            .WithMessage("El número de documento es obligatorio")
            .MinimumLength(8)
            .WithMessage("El documento debe tener al menos 8 caracteres")
            .MaximumLength(20)
            .WithMessage("El documento no puede tener más de 20 caracteres")
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("El documento solo puede contener letras y números");

        RuleFor(x => x.NewPin)
            .NotEmpty()
            .WithMessage("El nuevo PIN es obligatorio")
            .Length(3, 4)
            .WithMessage("El PIN debe tener 3 o 4 dígitos")
            .Matches(@"^\d{3,4}$")
            .WithMessage("El PIN solo puede contener dígitos");

        RuleFor(x => x.ConfirmNewPin)
            .NotEmpty()
            .WithMessage("La confirmación del nuevo PIN es obligatoria")
            .Equal(x => x.NewPin)
            .WithMessage("La confirmación del PIN no coincide con el nuevo PIN");
    }
}