using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;

namespace FincaFenix.EFCore.Services.CommandServices
{
    public class WorkOrderCommandService : FincaFenixContext, IWorkOrderCommandService
    {
        public async Task<int> SaveWorkOrder(WorkOrderEntity workOrder, RecipeEntity recipe, List<WorkOrderWorkedSectorEntity> workedSectors)
        {
            using var transaction = await Database.BeginTransactionAsync();

            try
            {
                // Guardar receta si existe
                if (recipe != null)
                {
                    Recipes.Add(recipe);
                    await SaveChangesAsync();

                    workOrder.RecipeId = recipe.Id;
                }

                // Guardar orden de trabajo
                WorkOrders.Add(workOrder);
                await SaveChangesAsync();

                // Guardar sectores trabajados (si los hay)
                if (workOrder.WorkedSectors != null && workOrder.WorkedSectors.Any())
                {
                    foreach (var sector in workOrder.WorkedSectors)
                    {
                        sector.WorkOrderId = workOrder.Id;
                    }

                    WorkOrderWorkedSectors.AddRange(workOrder.WorkedSectors);
                    await SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return workOrder.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
