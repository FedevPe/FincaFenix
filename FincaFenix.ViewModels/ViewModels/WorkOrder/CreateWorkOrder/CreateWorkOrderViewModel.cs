using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers.WorkOrder;

namespace FincaFenix.ViewModels.ViewModels.WorkOrder.CreateWorkOrder
{
    public class CreateWorkOrderViewModel(
        ICreateWorkOrderController woController,
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
            WorkOrder.TotalArea = OrderData.TotalAreaSectors;
            WorkOrder.Status = OrderData.StartDate > OrderData.CreatedDate ? "Pendiente" : "Activo";
            WorkOrder.SectorList = OrderData.SelectedSectors.ToList();

            if (RecipeData.Machine != null && RecipeData.Details.Count > 0)
            {
                WorkOrder.Recipe = new RecipeWorkOrderDTO
                {
                    MachineId = RecipeData.Machine.Id,
                    VolumeMachine = RecipeData.Machine.Capacity,
                    VolumeMachineUnit = RecipeData.Machine.CapacityUnit,
                    TRV = RecipeData.Machine.TRV,
                    Details = RecipeData.Details.Select(m => new DetailRecipeDTO
                    {
                        MaterialId = m.MaterialId,
                        AmountRequired = m.AmountRequired,
                        AmountRequiredUnit = m.AmountRequiredUnit,
                        PestDisease = m.PestDisease.ToUpper().Trim(),
                        Brand = m.Brand.ToUpper(),
                        EstimatedAmount = m.EstimatedAmount,
                        EstimatedAmountUnit = GetEstimatedAmounUnit(m.AmountRequiredUnit),

                    }).ToList()
                };
            }
            else
            {
                WorkOrder.Recipe = null;
            }

            var result = await woController.CreateWorkOrder(WorkOrder);

            if (!result.Success)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
                }
                return;
            }
        }
        private string GetEstimatedAmounUnit(string unit)
        {
            return unit?.ToLower() switch
            {
                "kg" => "kg",
                "gr" => "Kg",
                "lts" => "lts",
                "cc" => "lts",
                _ => "lts"
            };
        }
    }
}
