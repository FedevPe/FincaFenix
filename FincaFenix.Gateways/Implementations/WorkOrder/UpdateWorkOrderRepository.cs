using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.Gateways.Implementations.WorkOrder
{
    public class UpdateWorkOrderRepository(
        IUpdateWorkOrderCommand command) : IUpdateWorkOrderRepository
    {
        public Task<bool> UpdateWorkOrderAsync(UpdateWorkOrderDTO dto, RecipeEntity? mappedRecipe)
        {
            return command.UpdateWorkOrderAsync(dto, mappedRecipe);
        }

        public Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            return command.UpdateWorkOrderState(workOrderId, newStatus);
        }
    }
}
