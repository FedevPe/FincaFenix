using FincaFenix.UsesCases.UseCases.Material;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Material
{
    public class CreateMaterialCommandValidator : AbstractValidator<CreateMaterialCommand>
    {
        public CreateMaterialCommandValidator()
        {
            RuleFor(x => x.Dto.ArticleName)
                .NotEmpty().WithMessage("El nombre del artículo es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre del artículo no puede superar los 100 caracteres.");

            RuleFor(x => x.Dto.CommercialName)
                .NotEmpty().WithMessage("El nombre comercial es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre comercial no puede superar los 100 caracteres.");

            RuleFor(x => x.Dto.CategoryId)
                .GreaterThan(0).WithMessage("La categoría es obligatoria.");

            RuleFor(x => x.Dto.UnitOfMeasureId)
                .GreaterThan(0).WithMessage("La unidad de medida es obligatoria.");

            RuleFor(x => x.Dto.ReferenceCost)
                .GreaterThanOrEqualTo(0).When(x => x.Dto.ReferenceCost.HasValue)
                .WithMessage("El costo de referencia no puede ser negativo.");

            RuleFor(x => x.Dto.CurrencyId)
                .GreaterThan(0).WithMessage("La divisa es obligatoria.");
        }
    }
}
