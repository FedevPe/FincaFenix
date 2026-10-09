using FincaFenix.UsesCases.UseCases.WorkOrder;
using FluentValidation;

namespace FincaFenix.Validators.Validators.WorkOrder
{
    public class UpdateWorkOrderCommandValidator : AbstractValidator<UpdateWorkOrderCommand>
    {
        public UpdateWorkOrderCommandValidator()
        {
            RuleFor(x => x.WorkOrder.Id)
                .GreaterThan(0).WithMessage("La orden de trabajo es obligatoria.");

            RuleFor(x => x.WorkOrder.TotalArea)
                .GreaterThanOrEqualTo(0).When(x => x.WorkOrder.TotalArea.HasValue)
                .WithMessage("El área total no puede ser negativa.");
        }
    }
}
