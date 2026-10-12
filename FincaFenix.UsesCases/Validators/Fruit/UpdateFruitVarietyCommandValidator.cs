using FincaFenix.UsesCases.UseCases.Fruit;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Fruit
{
    public class UpdateFruitVarietyCommandValidator : AbstractValidator<UpdateFruitVarietyCommand>
    {
        public UpdateFruitVarietyCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("La variedad es obligatoria.")
                .GreaterThan(0).WithMessage("La variedad es obligatoria.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la variedad es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
