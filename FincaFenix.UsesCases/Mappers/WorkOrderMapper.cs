using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Aggregates
{
    public class WorkOrderMapper
    {
        public static WorkOrderEntity ToEntity(WorkOrderDTO workOrderDTO)
        {
            var newWorkOrder = new WorkOrderEntity
            {
                TaskId = workOrderDTO.TaskId,
                FarmId = workOrderDTO.FarmId,
                StartDate = workOrderDTO.StartDate,
                State = workOrderDTO.State,
                Description = workOrderDTO.Description,
                IsDeleted = false
            };

            return newWorkOrder;
        }
        public static RecipeEntity MapRecipe(RecipeWorkOrderDTO recipeDto)
        {
            return new RecipeEntity
            {
                Dosage = recipeDto.Dose,
                DosageUnit = recipeDto.DoseUnit,
                MachineId = recipeDto.MachineId,
                State = recipeDto.State,
                IsDeleted = false,
                DetailRecipeList = recipeDto.Details?.Select(dr => new DetailRecipeEntity
                {
                    MaterialId = dr.MaterialId,
                    AmountRequired = dr.AmountRequired,
                    AmountRequiredUnit = dr.AmountRequiredUnit,
                    EstimatedAmount = dr.EstimatedAmount,
                    EstimatedAmountUnit = dr.EstimatedAmountUnit
                }).ToList() ?? []
            };
        }

        public static List<WorkOrderWorkedSectorEntity> MapWorkedSectors(IEnumerable<DetailSectorFarmDTO> sectors)
        {
            return sectors.Select(s => new WorkOrderWorkedSectorEntity
            {
                SectorFarmId = s.Id
            }).ToList();
        }
    }
}
