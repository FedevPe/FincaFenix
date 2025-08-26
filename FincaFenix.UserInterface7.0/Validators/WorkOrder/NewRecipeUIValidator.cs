using FincaFenix.ViewModels.ViewModels.WorkOrder.CreateWorkOrder;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.WorkOrder
{
    public class NewRecipeUIValidator : AbstractValidator<NewRecipeViewModel>
    {
        public NewRecipeUIValidator() 
        {
            RuleFor(x => x.Machine.Id)
                .GreaterThan(0).WithMessage("Debe seleccionar una máquina válida.")
                .NotNull().WithMessage("El campo es obligatorio");
            RuleFor(x => x.Machine.Capacity)
                .GreaterThan(0).WithMessage("El volumen de la máquina debe ser mayor que cero.")
                .NotNull().WithMessage("El campo es obligatorio");
            RuleFor(x => x.Machine.CapacityUnit)
                .NotEmpty().WithMessage("La unidad de volumen de la máquina no puede estar vacía.")
                .NotNull().WithMessage("El campo es obligatorio");
            RuleFor(x => x.Machine.TRV)
                .GreaterThan(0).WithMessage("El volumen de la máquina debe ser mayor que cero.")
                .NotNull().WithMessage("El campo es obligatorio");
            RuleFor(x => x.Details)
                .NotNull().WithMessage("La lista de detalles no puede ser nula.")
                .Must(details => details.Any()).WithMessage("Debe agregar al menos un material a la receta.");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<NewRecipeViewModel>.CreateWithOptions((NewRecipeViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
