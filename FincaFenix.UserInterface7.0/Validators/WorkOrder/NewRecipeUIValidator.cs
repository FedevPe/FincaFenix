using FincaFenix.ViewModels.ViewModels;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.WorkOrder
{
    public class NewRecipeUIValidator : AbstractValidator<NewRecipeViewModel>
    {
        public NewRecipeUIValidator() 
        {
            RuleFor(x => x.MachineId)
                .GreaterThan(0).WithMessage("Debe seleccionar una máquina válida.");
            RuleFor(x => x.VolumeMachine)
                .GreaterThan(0).WithMessage("El volumen de la máquina debe ser mayor que cero.");
            RuleFor(x => x.VolumeMachineUnit)
                .NotEmpty().WithMessage("La unidad de volumen de la máquina no puede estar vacía.");
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
