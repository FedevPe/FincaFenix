using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.Enum;
using FincaFenix.UsesCases.Repository;
using FluentValidation;

namespace FincaFenix.Validators.Validators.WorkOrder
{
    public class WorkOrderValidator : AbstractValidator<WorkOrderDTO>
    {
        public WorkOrderValidator(
            ITaskRepository taskRepo,
            IFarmRepository farmRepo,
            IDetailSectorRepository sectorRepo)
        {
            RuleFor(wo => wo.TaskId)
                .GreaterThan(0).WithMessage("La orden de trabajo debe tener una Tarea relacionada.");
            RuleFor(dsf => dsf.TaskId)
                .MustAsync(async (id, CancellationToken) =>
                {
                    return await taskRepo.Exists(id);
                })
                .WithMessage("La tarea seleccionada no existe en la base de datos.");

            RuleFor(wo => wo.FarmId)
                .GreaterThan(0).WithMessage("La orden de trabajo debe tener una Finca relacionada.");
            RuleFor(dsf => dsf.FarmId)
                .MustAsync(async (id, CancellationToken) =>
                {
                    return await farmRepo.Exists(id);
                })
                .WithMessage("La tarea seleccionada no existe en la base de datos.");

            RuleFor(wo => wo.Status)
                .NotEmpty().WithMessage("Ocurrió un error al asignar el estado de la orden.");

            RuleFor(wo => wo.StartDate)
                .NotEmpty().WithMessage("La orden de trabajo debe tener una fecha de inicio.");

            RuleFor(wo => wo.SectorList)
                .NotNull().WithMessage("La lista de sectores no puede ser nula.")
                .Must(list => list != null && list.Any()).WithMessage("La orden de trabajo debe tener al menos un sector asociado.");

            RuleForEach(wo => wo.SectorList).SetValidator(new DetailSectorFarmValidator(sectorRepo))
                .WithMessage("Los sectores de la orden de trabajo no son válidos.");

            RuleFor(wo => wo)
                .MustAsync(async (wo, CancellationToken) =>
                {
                    if (!await taskRepo.Exists(wo.TaskId))
                    {
                        return true;
                    }

                    var task = await taskRepo.GetTaskById(wo.TaskId);

                    if (task?.RendimientoMode == RendimientoModeEnum.MaterialEfficiency)
                    {
                        return wo.Recipe?.Details is { Count: > 0 };
                    }

                    return true;
                })
                .WithMessage("La tarea seleccionada requiere una receta con al menos un material.");
        }
    }
}
