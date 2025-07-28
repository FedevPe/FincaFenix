using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class CreateWorkOrderViewModel(IWorkOrderController wo) : WorkOrderDTO
    {
        public async Task SetDataWorkOrder(NewRecipeViewModel NewRecipeViewModel, InfoNewWorkOrderViewModel OrderDataViewModel)
        {
            this.FarmId = OrderDataViewModel.SelectedFarmId;
            this.TaskId = OrderDataViewModel.SelectedTaskId;
            this.StartDate = OrderDataViewModel.StartDate;
            this.Description = Description;
            this.SectorList = OrderDataViewModel.SelectedSectors.ToList();

            if (NewRecipeViewModel.Machine != null && NewRecipeViewModel.Details.Count > 0)
            {
                this.Recipe = new RecipeWorkOrderDTO
                {
                    MachineId = NewRecipeViewModel.Machine.Id,
                    VolumeMachine = NewRecipeViewModel.Dosage,
                    VolumeMachineUnit = NewRecipeViewModel.DosageUnit,
                    Details = NewRecipeViewModel.Details.Select(m => new DetailRecipeDTO
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
                this.Recipe = null;
            }

            await SaveWorkOrder();
        }
        public async Task SaveWorkOrder()
        {
            await wo.CreateWorkOrder(this);
        }
    }
}
