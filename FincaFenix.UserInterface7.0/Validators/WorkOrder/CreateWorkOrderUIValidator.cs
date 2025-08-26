using FincaFenix.ViewModels.ViewModels.WorkOrder.CreateWorkOrder;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.WorkOrder
{
    public class CreateWorkOrderUIValidator : AbstractValidator<CreateWorkOrderViewModel>
    {
        public CreateWorkOrderUIValidator()
        {
            RuleFor(x => x.OrderData).SetValidator(new NewOrderDataUIValidator());
            RuleFor(x => x.RecipeData).SetValidator(new NewRecipeUIValidator());
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
