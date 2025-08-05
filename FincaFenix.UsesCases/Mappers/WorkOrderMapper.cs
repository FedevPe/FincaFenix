using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Aggregates
{
    public class WorkOrderMapper
    {
        public static WorkOrderEntity ToEntity(WorkOrderDTO workOrderDTO)
        {
            var newWorkOrder = new WorkOrderEntity()
            {
                TaskId = workOrderDTO.TaskId,
                FarmId = workOrderDTO.FarmId,
                CreatedDate = workOrderDTO.CreatedDate,
                StartDate = workOrderDTO.StartDate,
                Status = workOrderDTO.StartDate > workOrderDTO.CreatedDate ? "Pendiente" : "Activo",
                Description = workOrderDTO.Description,
                IsDeleted = false
            };

            if (workOrderDTO.Recipe != null)
            {
                newWorkOrder.Recipe = MapRecipe(workOrderDTO.Recipe); 
            }

            if (workOrderDTO.SectorList != null && workOrderDTO.SectorList.Any())
            {
                newWorkOrder.WorkedSectors = MapWorkedSectors(workOrderDTO.SectorList); 
            }

            return newWorkOrder;
        }

        private static RecipeEntity MapRecipe(RecipeWorkOrderDTO recipeDTO)
        {
            return new RecipeEntity
            {
                Dosage = recipeDTO.VolumeMachine,
                DosageUnit = recipeDTO.VolumeMachineUnit,
                MachineId = recipeDTO.MachineId,
                State = recipeDTO.Status,
                TRV = recipeDTO.TRV,
                IsDeleted = false,
                DetailRecipeList = recipeDTO.Details?.Select(dr => MapDetailRecipe(dr)).ToList() ?? []
            };
        }

        private static DetailRecipeEntity MapDetailRecipe(DetailRecipeDTO drDTO)
        {
            return new DetailRecipeEntity
            {
                MaterialId = drDTO.MaterialId,
                AmountRequired = drDTO.AmountRequired,
                AmountRequiredUnit = drDTO.AmountRequiredUnit,
                EstimatedAmount = drDTO.EstimatedAmount,
                EstimatedAmountUnit = drDTO.EstimatedAmountUnit,
                Brand = drDTO.Brand,
                DiseasePlague = drDTO.PestDisease
            };
        }

        private static List<WorkOrderWorkedSectorEntity> MapWorkedSectors(List<DetailSectorFarmDTO> sectorListDTO)
        {
            return sectorListDTO.Select(s => new WorkOrderWorkedSectorEntity
            {
                SectorFarmId = s.Id
            }).ToList();
        }
    }
}
