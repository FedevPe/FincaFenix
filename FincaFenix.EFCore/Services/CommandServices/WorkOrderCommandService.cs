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
                
                if (recipe != null)
                {
                    Recipes.Add(recipe);
                    await SaveChangesAsync();

                    workOrder.RecipeId = recipe.Id;
                }

                // Guardar orden de trabajo
                WorkOrders.Add(workOrder);
                await SaveChangesAsync();

               
                if (workedSectors != null && workedSectors.Any())
                {
                    foreach (var sector in workedSectors)
                    {
                        sector.WorkOrderId = workOrder.Id;
                    }

                    WorkOrderWorkedSectors.AddRange(workedSectors);
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
