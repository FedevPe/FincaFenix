using FincaFenix.UsesCases.UseCases.UnitOfMeasure;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Material
{
    public class UpdateUnitOfMeasureCommandValidator : AbstractValidator<UpdateUnitOfMeasureCommand>
    {
        public UpdateUnitOfMeasureCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("La unidad de medida es obligatoria.")
                .GreaterThan(0).WithMessage("La unidad de medida es obligatoria.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la unidad de medida es obligatoria.")
                .MaximumLength(50).WithMessage("La descripción no puede superar los 50 caracteres.");
        }
    }
}
