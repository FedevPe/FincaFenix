using FincaFenix.UsesCases.UseCases.Fruit;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Fruit
{
    public class UpdateFruitCommandValidator : AbstractValidator<UpdateFruitCommand>
    {
        public UpdateFruitCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("La fruta es obligatoria.")
                .GreaterThan(0).WithMessage("La fruta es obligatoria.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la fruta es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
