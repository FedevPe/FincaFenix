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
        public string WorkOrderNumber { get; private set; }
        public string RecipeNumber { get; private set; }

        public async Task Handle(WorkOrderDTO workOrder)
        {
            RecipeEntity recipeEntity = null;
            if (workOrder.Recipe != null)
            {
                RecipeNumber = await repository.GetLastNumberDoc("Receta");
                recipeEntity = WorkOrderMapper.MapRecipe(workOrder.Recipe, RecipeNumber);
            }

            // Paso 2: Obtener número de orden de trabajo
            WorkOrderNumber = await repository.GetLastNumberDoc("OrdenesTrabajo");

            // Paso 3: Mapear la orden de trabajo
            var workOrderEntity = WorkOrderMapper.ToEntity(workOrder, WorkOrderNumber);
            workOrderEntity.Recipe = recipeEntity;

            // Paso 4: Mapear sectores trabajados (si vienen)
            List<WorkOrderWorkedSectorEntity> workedSectors = [];

            if (workOrder.SectorList != null && workOrder.SectorList.Any())
            {
                workedSectors = WorkOrderMapper.MapWorkedSectors(workOrder.SectorList); // El ID se ajusta en repositorio
            }

            // Paso 5: Guardar en repositorio
            var idWorkOrder = await repository.AddWorkOrder(workOrderEntity, workOrderEntity.Recipe, workedSectors);

            // Paso 6: Presentar resultado
            await presenter.Handle(idWorkOrder);
        }
    }
}
    
