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
            var woNumber =  CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "OrdenTrabajo");
            var reNumber = CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "Receta");

            try
            {
                
                if (recipe != null)
                {
                    recipe.NumRecipe = reNumber.LastNumber.ToString();
                    Recipes.Add(recipe);
                    await SaveChangesAsync();

                    workOrder.RecipeId = recipe.Id;
                    reNumber.LastNumber++;
                    await SaveChangesAsync();
                }

                // Guardar orden de trabajo
                workOrder.OrderNum = woNumber.LastNumber.ToString();
                WorkOrders.Add(workOrder);
                await SaveChangesAsync();

                woNumber.LastNumber++;
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
