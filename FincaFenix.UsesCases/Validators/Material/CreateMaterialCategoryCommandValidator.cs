using FincaFenix.UsesCases.UseCases.MaterialCategory;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Material
{
    public class CreateMaterialCategoryCommandValidator : AbstractValidator<CreateMaterialCategoryCommand>
    {
        public CreateMaterialCategoryCommandValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la categoría es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");
        }
    }
}
