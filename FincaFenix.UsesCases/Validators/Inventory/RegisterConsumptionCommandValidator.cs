using FincaFenix.UsesCases.UseCases.Inventory;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Inventory
{
    public class RegisterConsumptionCommandValidator : AbstractValidator<RegisterConsumptionCommand>
    {
        public RegisterConsumptionCommandValidator()
        {
            RuleFor(x => x.Dto.WorkOrderId)
                .GreaterThan(0).WithMessage("La orden de trabajo es obligatoria.");

            RuleFor(x => x.Dto.MaterialId)
                .GreaterThan(0).WithMessage("El material es obligatorio.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        }
    }
}
