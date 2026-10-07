using FincaFenix.UsesCases.UseCases.WorkOrder;
using FincaFenix.Validators.Validators.WorkOrder;
using FluentValidation;

namespace FincaFenix.Validators.Validators.WorkOrder
{
    public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
    {
        public CreateWorkOrderCommandValidator(WorkOrderValidator workOrderValidator)
        {
            RuleFor(x => x.WorkOrder).SetValidator(workOrderValidator);
        }
    }
}
