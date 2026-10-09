using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.WorkOrder
{
    public interface IUpdateWorkOrderRepository
    {
        Task<bool> UpdateWorkOrderAsync(UpdateWorkOrderDTO dto, RecipeEntity? mappedRecipe);
        Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus);
    }
}
