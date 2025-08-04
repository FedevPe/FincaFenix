using FincaFenix.Entities.DTOs.RecipeDTO;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators
{
    public class DetailRecipeUIValidator : AbstractValidator<DetailRecipeDTO>
    {
        public DetailRecipeUIValidator()
        {
            RuleFor(dr => dr.CategoryId)
                .GreaterThan(0).WithMessage("La categoría seleccionada no es válida.");
            RuleFor(dr => dr.MaterialId)
                .GreaterThan(0).WithMessage("El producto seleccionado de la receta no es válido.");
            RuleFor(dr => dr.Brand)
                .NotEmpty().WithMessage("Ingrese la marca del producto.")
                .MaximumLength(100).WithMessage("Límite de caracteres excedido.");
            RuleFor(dr => dr.PestDisease)
                .NotEmpty().WithMessage("El campo plaga/enfermedad del producto no puede estar vacío.");
            RuleFor(dr => dr.AmountRequired)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad requerida del producto no puede ser menor o igual a 0.")
                .NotEmpty().WithMessage("Campo obligatorio");
            RuleFor(dr => dr.AmountRequiredUnit)
                .NotEmpty().WithMessage("Campo obligatorio");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DetailRecipeDTO>.CreateWithOptions((DetailRecipeDTO)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
