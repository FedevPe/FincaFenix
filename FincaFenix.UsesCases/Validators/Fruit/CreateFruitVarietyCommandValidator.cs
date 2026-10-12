using FincaFenix.UsesCases.UseCases.Fruit;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Fruit
{
    public class CreateFruitVarietyCommandValidator : AbstractValidator<CreateFruitVarietyCommand>
    {
        public CreateFruitVarietyCommandValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la variedad es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
