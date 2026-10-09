using FincaFenix.UsesCases.UseCases.MaterialCategory;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Material
{
    public class UpdateMaterialCategoryCommandValidator : AbstractValidator<UpdateMaterialCategoryCommand>
    {
        public UpdateMaterialCategoryCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("La categoría es obligatoria.")
                .GreaterThan(0).WithMessage("La categoría es obligatoria.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la categoría es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
