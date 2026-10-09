using FincaFenix.Entities.Enum;
using FincaFenix.UsesCases.Repository.WorkOrder;
using FincaFenix.UsesCases.UseCases.DetailWorkOrder;
using FluentValidation;

namespace FincaFenix.Validators.Validators.DetailWorkOrder
{
    public class AddDetailWorkOrderCommandValidator : AbstractValidator<AddDetailWorkOrderCommand>
    {
        public AddDetailWorkOrderCommandValidator(IGetWorkOrderInformationRepository workOrderRepository)
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

            RuleFor(x => x.Dto.Info.MachinePasses)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de maquinadas no puede ser negativa.");

            RuleFor(x => x.Dto.Info.ProducedAmount)
                .GreaterThanOrEqualTo(0).When(x => x.Dto.Info.ProducedAmount.HasValue)
                .WithMessage("La cantidad producida no puede ser negativa.");

            RuleFor(x => x.Dto.Info.Description)
                .NotEmpty().WithMessage("La descripción de la actividad es obligatoria.");

            RuleFor(x => x.Dto)
                .CustomAsync(async (dto, context, cancellationToken) =>
                {
                    var workOrder = await workOrderRepository.GetWorkOrderAndRecipeByIdWorkorder(dto.OrderId);

                    if (workOrder is null)
                    {
                        context.AddFailure("La orden de trabajo no existe.");
                        return;
                    }

                    var mode = workOrder.Task?.RendimientoMode ?? RendimientoModeEnum.ManHours;

                    switch (mode)
                    {
                        case RendimientoModeEnum.MaterialEfficiency:
                            if (workOrder.Recipe?.DetailRecipeList is not { Count: > 0 })
                            {
                                context.AddFailure("La tarea seleccionada requiere una receta asociada a la orden de trabajo.");
                            }

                            if (dto.Info.MachinePasses <= 0)
                            {
                                context.AddFailure("La cantidad de maquinadas debe ser mayor a cero.");
                            }
                            break;

                        case RendimientoModeEnum.OutputPerManHour:
                            if ((dto.Info.ProducedAmount ?? 0m) <= 0)
                            {
                                context.AddFailure("La cantidad producida (kg) debe ser mayor a cero.");
                            }
                            break;
                    }
                });
        }
    }
}
