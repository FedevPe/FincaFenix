using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.Gateways.Implementations.WorkOrder
{
    public class CreateWorkOrderRepository(
        ICorrelativeNumberService correlativeNumberService,
        IRecipeCommand recipeCommand,
        IWorkOrderCommand workOrderCommand,
        IWorkOrderInventoryCommand workOrderInventoryCommand,
        IUnitOfWork unitOfWork) : ICreateWorkOrderRepository
    {
        public async Task<int> CreateWorkOrder(WorkOrderEntity workOrder)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var workOrderCorrelative = await correlativeNumberService.GetByTypeDoc("OrdenTrabajo");
                var recipeCorrelative = await correlativeNumberService.GetByTypeDoc("Receta");

                if (workOrder.Recipe != null)
                {
                    workOrder.Recipe.NumRecipe = recipeCorrelative.LastNumber.ToString();
                    await recipeCommand.AddRecipe(workOrder.Recipe);
                    recipeCorrelative.LastNumber++;
                    await unitOfWork.SaveChangesAsync();

                    workOrder.RecipeId = workOrder.Recipe.Id;
                }

                workOrder.OrderNum = workOrderCorrelative.LastNumber.ToString();
                await workOrderCommand.AddWorkOrder(workOrder);
                workOrderCorrelative.LastNumber++;
                await unitOfWork.SaveChangesAsync();

                if (workOrder.Recipe?.DetailRecipeList is { Count: > 0 })
                {
                    await workOrderInventoryCommand.RegisterReservationsAndCostsAsync(workOrder, workOrder.Recipe.DetailRecipeList);
                }

                await unitOfWork.CommitAsync();
                return workOrder.Id;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
