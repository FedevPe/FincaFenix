using FincaFenix.ViewModels.ViewModels;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators
{
    public class CreateWorkOrderValidator : AbstractValidator<CreateWorkOrderViewModel>
    {
        public CreateWorkOrderValidator()
        {
            RuleFor(x => x.OrderData).SetValidator(new NewOrderDataValidator());
            RuleFor(x => x.RecipeData).SetValidator(new NewRecipeValidator());
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CreateWorkOrderViewModel>.CreateWithOptions((CreateWorkOrderViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
