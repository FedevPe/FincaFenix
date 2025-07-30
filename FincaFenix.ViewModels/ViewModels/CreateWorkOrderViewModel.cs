using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class CreateWorkOrderViewModel(
        IWorkOrderController woController,
        NewOrderDataViewModel orderData,
        NewRecipeViewModel recipeData)
    {
        public WorkOrderDTO WorkOrder { get; set; } = new WorkOrderDTO();
        public NewOrderDataViewModel OrderData { get; set; } = orderData;
        public NewRecipeViewModel RecipeData { get; set; } = recipeData;


        public async Task SetDataWorkOrder()
        {
            WorkOrder.FarmId = OrderData.SelectedFarmId;
            WorkOrder.TaskId = OrderData.SelectedTaskId;
            WorkOrder.StartDate = OrderData.StartDate;
            WorkOrder.Status = OrderData.StartDate > OrderData.CreatedDate ? "Pendiente" : "Activo";
            WorkOrder.SectorList = OrderData.SelectedSectors.ToList();

            if (RecipeData.Machine != null && RecipeData.Details.Count > 0)
            {
                WorkOrder.Recipe = new RecipeWorkOrderDTO
                {
                    MachineId = RecipeData.Machine.Id,
                    VolumeMachine = RecipeData.VolumeMachine,
                    VolumeMachineUnit = RecipeData.VolumeMachineUnit,
                    Details = RecipeData.Details.Select(m => new DetailRecipeDTO
                    {
                        MaterialId = m.MaterialId,
                        AmountRequired = m.AmountRequired,
                        AmountRequiredUnit = m.AmountRequiredUnit,
                        EstimatedAmount = m.EstimatedAmount,
                        EstimatedAmountUnit = m.AmountRequiredUnit
                    }).ToList()
                };
            }
            else
            {
                WorkOrder.Recipe = null;
            }

            await woController.CreateWorkOrder(WorkOrder);
        }
    }
}
