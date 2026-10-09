using FincaFenix.UsesCases.UseCases.UnitOfMeasure;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Material
{
    public class CreateUnitOfMeasureCommandValidator : AbstractValidator<CreateUnitOfMeasureCommand>
    {
        public CreateUnitOfMeasureCommandValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la unidad de medida es obligatoria.")
                .MaximumLength(50).WithMessage("La descripción no puede superar los 50 caracteres.");
        }
    }
}
