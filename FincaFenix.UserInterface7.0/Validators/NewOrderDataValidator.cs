using FincaFenix.ViewModels.ViewModels;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators
{
    public class NewOrderDataValidator : AbstractValidator<NewOrderDataViewModel>
    {
        public NewOrderDataValidator()
        {
            RuleFor(x => x.SelectedFarmId)
                .GreaterThan(0).WithMessage("Debe seleccionar una finca válida.");
            RuleFor(x => x.SelectedTaskId)
                .GreaterThan(0).WithMessage("Debe seleccionar una tarea válida.");
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("La fecha de inicio no puede estar vacía.");
            RuleFor(x => x.CreatedDate)
                .NotEmpty().WithMessage("La fecha de creación no puede estar vacía.");
            RuleFor(x => x.SelectedSectors)
                .NotNull().WithMessage("Debe seleccionar al menos un sector.")
                .Must(sectors => sectors.Any()).WithMessage("Debe seleccionar al menos un sector.");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<NewOrderDataViewModel>.CreateWithOptions((NewOrderDataViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
