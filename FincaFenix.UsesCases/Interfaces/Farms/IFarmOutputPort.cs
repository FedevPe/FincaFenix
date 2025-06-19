using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Farms
{
    public interface IFarmOutputPort
    {
        Task Handle(TaskDTO task);
        Task HandleList(List<TaskDTO> materialList);
    }
}
