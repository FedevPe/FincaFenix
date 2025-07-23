using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;

namespace FincaFenix.EFCore.Services.CommandServices
{
    public class WorkOrderCommandService : FincaFenixContext, IWorkOrderCommandService
    {
        public async Task<int> SaveWorkOrder(WorkOrderEntity workOrder)
        {
            using var transaction = await Database.BeginTransactionAsync();
            var woNumber =  CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "OrdenTrabajo");
            var reNumber = CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "Receta");

            try
            {
                
                if (workOrder.Recipe != null)
                {
                    workOrder.Recipe.NumRecipe = reNumber.LastNumber.ToString();
                    Recipes.Add(workOrder.Recipe);
                    await SaveChangesAsync();

                    workOrder.RecipeId = workOrder.Recipe.Id;
                    reNumber.LastNumber++;
                    await SaveChangesAsync();
                }

                // Guardar orden de trabajo
                workOrder.OrderNum = woNumber.LastNumber.ToString();
                WorkOrders.Add(workOrder);
                await SaveChangesAsync();

                woNumber.LastNumber++;
                await SaveChangesAsync();

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
