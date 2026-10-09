using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface IUpdateWorkOrderCommand
    {
        Task<bool> UpdateWorkOrderAsync(UpdateWorkOrderDTO dto, RecipeEntity? mappedRecipe);
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
    }
}
