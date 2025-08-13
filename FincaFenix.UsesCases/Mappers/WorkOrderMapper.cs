using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using System.Net.NetworkInformation;

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
                VolumeMachine = recipeDTO.VolumeMachine,
                VolumeMachineUnit = recipeDTO.VolumeMachineUnit,
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
        public static ShowWorkOrderDTO EntityToDTO(WorkOrderEntity workOrderEntity)
        {
            return new ShowWorkOrderDTO()
            {
                Id = workOrderEntity.Id,
                OrderNum = workOrderEntity.OrderNum,
                Status = workOrderEntity.Status,
                Description = workOrderEntity.Description,
                CreatedDate = workOrderEntity.CreatedDate,
                StartDate = workOrderEntity.StartDate,
                EndDate = workOrderEntity.EndDate,
                IsDeleted = workOrderEntity.IsDeleted,
                Task = new TaskDTO
                {
                    Id = workOrderEntity.Task.Id,
                    Description = workOrderEntity.Task.Description
                },
                Farm = new FarmDTO
                {
                    Id = workOrderEntity.Farm.Id,
                    Name = workOrderEntity.Farm.Name
                },
                SectorList = workOrderEntity.WorkedSectors.Select(sector => new DetailSectorFarmDTO
                {
                    Id = sector.SectorFarmId,
                    SectorName = sector.SectorFarm.SectorName,
                    Area = sector.SectorFarm.Area,
                    NumberPlants = sector.SectorFarm.NumberPlants,
                    VarietyName = sector.SectorFarm.Variety.Description,
                    FruitName = sector.SectorFarm.Variety.Fruit.Description,
                }),
                Recipe = workOrderEntity.Recipe != null ? new RecipeWorkOrderDTO
                {
                    Id = workOrderEntity.Recipe.Id,
                    NumRecipe = workOrderEntity.Recipe.NumRecipe,
                    Machine = workOrderEntity.Recipe.Machine != null ? new MachineRecipeDTO
                    {
                        Id = workOrderEntity.Recipe.Machine.Id,
                        Name = workOrderEntity.Recipe.Machine.Name,
                        Capacity = workOrderEntity.Recipe.Machine.Capacity,
                        CapacityUnit = workOrderEntity.Recipe.Machine.CapacityUnit
                    } : null,
                    VolumeMachine = workOrderEntity.Recipe.VolumeMachine,
                    VolumeMachineUnit = workOrderEntity.Recipe.VolumeMachineUnit,
                    TRV = workOrderEntity.Recipe.TRV,
                    IsDeleted = workOrderEntity.Recipe.IsDeleted,
                    Details = workOrderEntity.Recipe.DetailRecipeList.Select(detail => new DetailRecipeDTO
                    {
                        Material = new MaterialRecipeDTO
                        {
                            Id = detail.Material.Id,
                            ArticleName = detail.Material.ArticleName,
                            CommercialName = detail.Material.CommercialName,
                        },
                        Brand = detail.Brand,
                        PestDisease = detail.DiseasePlague,
                        AmountRequired = detail.AmountRequired,
                        AmountRequiredUnit = detail.AmountRequiredUnit,
                        EstimatedAmount = detail.EstimatedAmount,
                        EstimatedAmountUnit = detail.EstimatedAmountUnit
                    }).ToList()
                } : null,
                DetailsWorkOrder = AddDetailWorkOrder(workOrderEntity.DetailWorkOrderList)
            };
        }
        private static IEnumerable<DetailWorkOrderDTO> AddDetailWorkOrder(IEnumerable<DetailWorkOrderEntity> details)
        {
            if (details != null && details.Any())
            {
                IEnumerable<DetailWorkOrderDTO> detailsWorkOrderDTO = details.Select(detail => new DetailWorkOrderDTO
                {
                    Employee = new EmployeeDTO
                    {
                        Id = detail.Employee.Id,
                        Name = detail.Employee.Name,
                        LastName = detail.Employee.LastName
                    },
                    Sector = new DetailSectorFarmDTO
                    {
                        SectorName = detail.SectorWorked.SectorName
                    },
                    Performance = detail.Performance,
                    WorkedHours = detail.WorkedHours,
                    Description = detail.Description,
                    ActivityDate = detail.ActivityDate

                });
                return detailsWorkOrderDTO;
            }
            else
            {
                return null;
            }
        }
    }
}
