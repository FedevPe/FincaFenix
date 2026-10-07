using FincaFenix.UsesCases.UseCases.DetailWorkOrder;
using FluentValidation;

namespace FincaFenix.Validators.Validators.DetailWorkOrder
{
    public class AddDetailWorkOrderCommandValidator : AbstractValidator<AddDetailWorkOrderCommand>
    {
        public AddDetailWorkOrderCommandValidator()
        {
            RuleFor(x => x.Dto.OrderId)
                .GreaterThan(0).WithMessage("La orden de trabajo es obligatoria.");

            RuleFor(x => x.Dto.EmployeeId)
                .GreaterThan(0).WithMessage("El empleado es obligatorio.");

            RuleFor(x => x.Dto.ActivityDate)
                .NotNull().WithMessage("La fecha de actividad es obligatoria.");

            RuleFor(x => x.Dto.Info.SectorWorkedId)
                .GreaterThan(0).WithMessage("El sector trabajado es obligatorio.");

            RuleFor(x => x.Dto.Info.WorkedHours)
                .GreaterThan(0).WithMessage("Las horas trabajadas deben ser mayores a cero.");

            RuleFor(x => x.Dto.Info.Performance)
                .GreaterThanOrEqualTo(0).WithMessage("El rendimiento no puede ser negativo.");

            RuleFor(x => x.Dto.Info.Description)
                .NotEmpty().WithMessage("La descripción de la actividad es obligatoria.");
        }
    }
}
