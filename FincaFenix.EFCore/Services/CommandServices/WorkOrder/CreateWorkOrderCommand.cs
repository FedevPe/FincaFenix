using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class CreateWorkOrderCommand(FincaFenixContext context) : ICreateWorkOrderCommand
    {
        public async Task<int> AddNewWorkOrder(WorkOrderEntity workOrder)
        {
            var transaction = await context.Database.BeginTransactionAsync();
            var woNumber = await context.CorrelativeNumber.FirstOrDefaultAsync(c => c.TypeDoc == "OrdenTrabajo");
            var reNumber = await context.CorrelativeNumber.FirstOrDefaultAsync(c => c.TypeDoc == "Receta");

            try
            {
                if (workOrder.Recipe != null)
                {
                    workOrder.Recipe.NumRecipe = reNumber.LastNumber.ToString();
                    context.Recipes.Add(workOrder.Recipe);
                    reNumber.LastNumber++;
                    await context.SaveChangesAsync();

                    workOrder.RecipeId = workOrder.Recipe.Id;
                }

                workOrder.OrderNum = woNumber.LastNumber.ToString();
                context.WorkOrders.Add(workOrder);
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
