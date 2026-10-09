using FincaFenix.Entities.Enum;
using FincaFenix.UsesCases.UseCases.Inventory;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Inventory
{
    public class RegisterMovementCommandValidator : AbstractValidator<RegisterMovementCommand>
    {
        public RegisterMovementCommandValidator()
        {
            RuleFor(x => x.Dto.MaterialId)
                .GreaterThan(0).WithMessage("El material es obligatorio.");

            RuleFor(x => x.Dto.FarmId)
                .GreaterThan(0).WithMessage("La finca es obligatoria.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");

            RuleFor(x => x.Dto.UnitCost)
                .GreaterThanOrEqualTo(0).When(x => x.Dto.UnitCost.HasValue)
                .WithMessage("El costo unitario no puede ser negativo.");

            RuleFor(x => x.Dto.StockMinimum)
                .GreaterThanOrEqualTo(0).When(x => x.Dto.StockMinimum.HasValue)
                .WithMessage("El stock mínimo no puede ser negativo.");

            RuleFor(x => x.Dto.MovementType)
                .Must(BeAValidManualMovement).WithMessage("El tipo de movimiento no es válido.");

            RuleFor(x => x.Dto.CurrencyId)
                .GreaterThan(0).WithMessage("La divisa es obligatoria.");
        }

        private static bool BeAValidManualMovement(string movementType)
        {
            if (string.IsNullOrWhiteSpace(movementType))
                return false;

            return movementType.Equals(InventoryMovementTypeEnum.Ingreso.ToString())
                || movementType.Equals(InventoryMovementTypeEnum.AjustePositivo.ToString())
                || movementType.Equals(InventoryMovementTypeEnum.AjusteNegativo.ToString());
        }
    }
}
