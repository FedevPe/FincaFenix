using FincaFenix.ViewModels.ViewModels;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.DetailWorkOrder
{
    public class RegistryActivityViewModelValidator : AbstractValidator<RegistryActivityViewModel>
    {
        public RegistryActivityViewModelValidator()
        {
            RuleFor(vm => vm.EmployeeId)
                .GreaterThan(0).WithMessage("Debe seleccionar un operario.")
                .NotNull().WithMessage("Debe seleccionar un operario.");
            RuleFor(vm => DateOnly.FromDateTime(vm.ActivityDate.Value))
                .NotNull().WithMessage("La fecha de es obligatoria.")
                .Equal(DateOnly.FromDateTime(DateTime.Now)).WithMessage("La fecha seleccionada no es válida.");


        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<RegistryActivityViewModel>.CreateWithOptions((RegistryActivityViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
