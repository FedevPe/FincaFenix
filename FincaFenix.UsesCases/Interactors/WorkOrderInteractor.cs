using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Aggregates;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.WorkOrder;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class WorkOrderInteractor(
        IWorkOrderOutputPort presenter,
        IWorkOrderRepository repository) : IWorkOrderInputPort
    {
        public async Task GetWorkOrderById(int id)
        {
            await presenter.Handle(await repository.GetWorkOrderById(id));
        }
        public async Task GetWorkOrderListPaginated(int pageNumber, int pageSize)
        {
            var (workOrders, totalCount) = await repository.GetWorkOrderList(pageNumber, pageSize);
            await presenter.HandleList(workOrders, totalCount);
        }
        public async Task Handle(WorkOrderDTO workOrder)
        {
            RecipeEntity recipeEntity = null;
            if (workOrder.Recipe != null)
            {
                recipeEntity = WorkOrderMapper.MapRecipe(workOrder.Recipe);
            }

            var workOrderEntity = WorkOrderMapper.ToEntity(workOrder);
            workOrderEntity.Recipe = recipeEntity;

            List<WorkOrderWorkedSectorEntity> workedSectors = [];

            if (workOrder.SectorList != null && workOrder.SectorList.Any())
            {
                workedSectors = WorkOrderMapper.MapWorkedSectors(workOrder.SectorList);
            }

            var idWorkOrder = await repository.AddWorkOrder(workOrderEntity, workOrderEntity.Recipe, workedSectors);

            await presenter.Handle(idWorkOrder);
        }
    }
}
