using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class CreateWorkOrderCommand : FincaFenixContext, ICreateWorkOrderCommand
    {
        public async Task<int> AddNewWorkOrder(WorkOrderEntity workOrder)
        {
            var transaction = await Database.BeginTransactionAsync();
            var woNumber = CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "OrdenTrabajo");
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
