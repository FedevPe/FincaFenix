using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.WorkOrder
{
    public class NewDetailSectorUIValidator : AbstractValidator<DetailSectorFarmDTO>
    {
        public NewDetailSectorUIValidator()
        {
            RuleFor(s => s.Id)
                .GreaterThan(0).WithMessage("El sector seleccionado no es válido.")
                .NotNull().WithMessage("Seleccione un cuadro.");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<DetailSectorFarmDTO>.CreateWithOptions((DetailSectorFarmDTO)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
