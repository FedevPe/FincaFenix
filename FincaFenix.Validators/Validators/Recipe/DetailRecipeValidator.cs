using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.UsesCases.Repository;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Recipe
{
    public class DetailRecipeValidator : AbstractValidator<DetailRecipeDTO>
    {
        public DetailRecipeValidator(
            IMaterialCategoryRepository categoryRepository,
            IMaterialRepository materialRepository)
        {
            RuleFor(dr => dr.CategoryId)
                .GreaterThan(0).WithMessage("La categoría seleccionada de la receta no es válida.");
            RuleFor(dr => dr.CategoryId)
                .MustAsync(async (id, CancellationToken) =>
                {
                    return await categoryRepository.Exists(id);
                })
                .WithMessage("La categoría seleccionada de la receta no existe en la base de datos.");

            RuleFor(dr => dr.MaterialId)
                .GreaterThan(0).WithMessage("El material seleccionado de la receta no es válido.");
            RuleFor(dr => dr.MaterialId)
                .MustAsync(async (id, CancellationToken) =>
                {
                    return await materialRepository.Exists(id);
                })
                .WithMessage("El material seleccionado de la receta no existe en la base de datos.");

            RuleFor(dr => dr.AmountRequired)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad requerida del material no puede ser menor o igual a 0.")
                .NotEmpty().WithMessage("La cantidad requerida del material no puede estar vacía.");
            RuleFor(dr => dr.AmountRequiredUnit)
                .NotEmpty().WithMessage("La unidad de cantidad requerida del material no puede estar vacía.");

            RuleFor(dr => dr.EstimatedAmount)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad estimada del material no puede ser menor o igual a 0.")
                .NotEmpty().WithMessage("La cantidad estimada del material no puede estar vacía.");
            RuleFor(dr => dr.EstimatedAmountUnit)
                .NotEmpty().WithMessage("La unidad de cantidad requerida del material no puede estar vacía.");

        }
    }
}
