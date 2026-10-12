using FincaFenix.UsesCases.UseCases.Fruit;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Fruit
{
    public class CreateFruitCommandValidator : AbstractValidator<CreateFruitCommand>
    {
        public CreateFruitCommandValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la fruta es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
