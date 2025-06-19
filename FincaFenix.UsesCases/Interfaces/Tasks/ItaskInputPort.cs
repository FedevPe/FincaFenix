using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Tasks
{
    public interface ItaskInputPort
    {
        Task Handle(TaskOrderDTO  task);
    }
}
