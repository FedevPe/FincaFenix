using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class CreateWorkOrderCommand(FincaFenixContext context) : ICreateWorkOrderCommand
    {
        public async Task<int> AddNewWorkOrder(WorkOrderEntity workOrder)
        {
            var transaction = await context.Database.BeginTransactionAsync();
            var woNumber = context.CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "OrdenTrabajo");
            var reNumber = context.CorrelativeNumber.FirstOrDefault(c => c.TypeDoc == "Receta");

            try
            {
                if (workOrder.Recipe != null)
                {
                    workOrder.Recipe.NumRecipe = reNumber.LastNumber.ToString();
                    context.Recipes.Add(workOrder.Recipe);
                    await context.SaveChangesAsync();

                    workOrder.RecipeId = workOrder.Recipe.Id;
                    reNumber.LastNumber++;
                    await context.SaveChangesAsync();
                }

                // Guardar orden de trabajo
                workOrder.OrderNum = woNumber.LastNumber.ToString();
                context.WorkOrders.Add(workOrder);
                await context.SaveChangesAsync();

                woNumber.LastNumber++;
                await context.SaveChangesAsync();

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
