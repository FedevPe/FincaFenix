using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.RegisterActivity
{
    public class DetailActivityUIValidator : AbstractValidator<AddDetailWorkOrderDTO>
    {
        public DetailActivityUIValidator()
        {
            RuleFor(ad => ad.Info.SectorWorkedId)
                .GreaterThan(0).WithMessage("Debe seleccionar un operario.")
                .NotNull().WithMessage("Debe seleccionar un operario.");
            RuleFor(ad => ad.Info.Performance)
                .GreaterThan(0).WithMessage("El rendimiento debe ser mayor que 0.")
                .NotNull().WithMessage("El rendimiento es obligatorio.");
            RuleFor(ad => ad.Info.WorkedHours)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que 0.")
                .LessThanOrEqualTo(9).WithMessage("La cantidad no puede ser mayor a 9 horas.")
                .NotNull().WithMessage("La cantidad es obligatoria.");
            RuleFor(ad => ad.Info.Description).Length(0, 200)
                .WithMessage("La descripción no puede exceder los 200 caracteres.");
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<AddDetailWorkOrderDTO>.CreateWithOptions((AddDetailWorkOrderDTO)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
