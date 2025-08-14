using FincaFenix.Entities.DTOs.RecipeDTO;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.WorkOrder
{
    public class RecipeMachineUIValidator : AbstractValidator<MachineRecipeDTO>
    {
        public RecipeMachineUIValidator()
        {
            RuleFor(mr => mr.Id)
                .NotEmpty().WithMessage("Este campo es obligatorio.")
                .GreaterThan(0).WithMessage("Este campo es obligatorio.");
            RuleFor(mr => mr.Capacity)
                .GreaterThan(0).WithMessage("La capacidad de la máquina debe ser mayor a 0.")
                .NotEmpty().WithMessage("La capacidad de la máquina es obligatoria.");
            RuleFor(mr => mr.CapacityUnit)
                .NotEmpty().WithMessage("La unidad de medida de la capacidad es obligatoria.");
            RuleFor(mr => mr.TRV)
                .GreaterThan(0).WithMessage("El TRV debe ser mayor 0.")
                .NotEmpty().WithMessage("El TRV es obligatorio.");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<MachineRecipeDTO>.CreateWithOptions((MachineRecipeDTO)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
