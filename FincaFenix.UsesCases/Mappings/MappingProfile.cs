using AutoMapper;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FarmEntity, FarmDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

        CreateMap<TaskEntity, TaskDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description));

        CreateMap<MachineEntity, MachineRecipeDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.Capacity, o => o.MapFrom(s => s.Capacity))
            .ForMember(d => d.CapacityUnit, o => o.MapFrom(s => s.CapacityUnit))
            .ForMember(d => d.TRV, o => o.Ignore());

        CreateMap<MaterialCategoryEntity, MaterialCategoryDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description));

        CreateMap<MaterialEntity, MaterialRecipeDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ArticleName, o => o.MapFrom(s => s.ArticleName))
            .ForMember(d => d.CommercialName, o => o.MapFrom(s => s.CommercialName))
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category != null ? new MaterialCategoryDTO
            {
                Id = s.Category.Id,
                Description = s.Category.Description
            } : null))
            .ForMember(d => d.UnitOfMeasure, o => o.MapFrom(s => s.UnitOfMeasure != null ? s.UnitOfMeasure.Description : null))
            .ForMember(d => d.DiseasePlague, o => o.Ignore());

        #region CRUD Materiales / Categorías / Unidades de medida

        CreateMap<MaterialEntity, MaterialDTO>()
            .ForMember(d => d.UnitOfMeasure, o => o.MapFrom(s => s.UnitOfMeasure != null ? s.UnitOfMeasure.Description : null))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.Currency != null ? s.Currency.Code : null));

        CreateMap<CreateMaterialDTO, MaterialEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.UnitOfMeasure, o => o.Ignore())
            .ForMember(d => d.Currency, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.MapFrom(s => false))
            .ForMember(d => d.DiseasePlagueMaterialList, o => o.Ignore());

        CreateMap<UpdateMaterialDTO, MaterialEntity>()
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.UnitOfMeasure, o => o.Ignore())
            .ForMember(d => d.Currency, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.DiseasePlagueMaterialList, o => o.Ignore());

        CreateMap<UnitOfMeasureEntity, UnitOfMeasureDTO>();

        CreateMap<SaveUnitOfMeasureDTO, UnitOfMeasureEntity>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id ?? 0))
            .ForMember(d => d.IsDeleted, o => o.Ignore());

        CreateMap<SaveMaterialCategoryDTO, MaterialCategoryEntity>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id ?? 0));

        #endregion

        CreateMap<DetailSectorFarmEntity, DetailSectorFarmDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.SectorName, o => o.MapFrom(s => s.SectorName))
            .ForMember(d => d.Area, o => o.MapFrom(s => s.Area))
            .ForMember(d => d.NumberPlants, o => o.MapFrom(s => s.NumberPlants))
            .ForMember(d => d.VarietyName, o => o.MapFrom(s => s.Variety != null ? s.Variety.Description : null))
            .ForMember(d => d.FruitName, o => o.MapFrom(s => s.Variety != null && s.Variety.Fruit != null ? s.Variety.Fruit.Description : null))
            .ForMember(d => d.Selected, o => o.Ignore());
        ;

        CreateMap<EmployeeEntity, EmployeeDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

        #region Fase 3.3 — WorkOrder (lectura: Entity -> DTO)

        CreateMap<WorkOrderEntity, ShowWorkOrderDTO>()
            .ForMember(d => d.SectorList, o => o.MapFrom(s => s.WorkedSectors.Select(w => w.SectorFarm)))
            .ForMember(d => d.DetailsWorkOrder, o => o.MapFrom(s => s.DetailWorkOrderList != null && s.DetailWorkOrderList.Any()
                ? s.DetailWorkOrderList
                : null));

        CreateMap<RecipeEntity, RecipeWorkOrderDTO>()
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.TotalAplications, o => o.Ignore())
            .ForMember(d => d.Details, o => o.MapFrom(s => s.DetailRecipeList));

        CreateMap<DetailRecipeEntity, DetailRecipeDTO>()
            .ForMember(d => d.CategoryId, o => o.Ignore())
            .ForMember(d => d.PestDisease, o => o.MapFrom(s => s.DiseasePlague))
            .ForMember(d => d.TotalAmountConsumed, o => o.Ignore());

        CreateMap<DetailWorkOrderEntity, ActivityWorkOrderDTO>()
            .ForMember(d => d.WorkOrder, o => o.Ignore())
            .ForMember(d => d.Sector, o => o.MapFrom(s => s.SectorWorked));

        #endregion

        #region Fase 3.3 — WorkOrder (escritura: DTO -> Entity)

        CreateMap<WorkOrderDTO, WorkOrderEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.OrderNum, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Task, o => o.Ignore())
            .ForMember(d => d.Farm, o => o.Ignore())
            .ForMember(d => d.EndDate, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())
            .ForMember(d => d.DetailWorkOrderList, o => o.Ignore())
            .ForMember(d => d.Status, o => o.MapFrom(s => s.StartDate > s.CreatedDate ? "Pendiente" : "Activo"))
            .ForMember(d => d.TotalAreaWorked, o => o.MapFrom(s => s.TotalArea.Value))
            .ForMember(d => d.IsDeleted, o => o.MapFrom(s => false))
            .ForMember(d => d.Recipe, o => o.MapFrom((s, d, member, ctx) => s.Recipe != null ? MapRecipeToEntity(s.Recipe, ctx) : null))
            .ForMember(d => d.WorkedSectors, o => o.MapFrom(s => s.SectorList == null || !s.SectorList.Any()
                ? new List<WorkOrderWorkedSectorEntity>()
                : s.SectorList.Select(x => new WorkOrderWorkedSectorEntity { SectorFarmId = x.Id }).ToList()));

        CreateMap<DetailRecipeDTO, DetailRecipeEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.RecipeId, o => o.Ignore())
            .ForMember(d => d.Recipe, o => o.Ignore())
            .ForMember(d => d.Material, o => o.Ignore())
            .ForMember(d => d.DiseasePlague, o => o.MapFrom(s => s.PestDisease))
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<RecipeWorkOrderDTO, RecipeEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.NumRecipe, o => o.Ignore())
            .ForMember(d => d.Machine, o => o.Ignore())
            .ForMember(d => d.State, o => o.MapFrom(s => s.Status))
            .ForMember(d => d.IsDeleted, o => o.MapFrom(s => false))
            .ForMember(d => d.RowVersion, o => o.Ignore())
            .ForMember(d => d.DetailRecipeList, o => o.MapFrom((s, d, member, ctx) =>
                GroupItems(s.Details ?? new List<DetailRecipeDTO>())
                    .Select(dr => ctx.Mapper.Map<DetailRecipeEntity>(dr)).ToList()));

        CreateMap<AddDetailWorkOrderDTO, DetailWorkOrderEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.WorkOrderId, o => o.MapFrom(s => s.OrderId))
            .ForMember(d => d.WorkOrder, o => o.Ignore())
            .ForMember(d => d.Employee, o => o.Ignore())
            .ForMember(d => d.SectorWorked, o => o.Ignore())
            .ForMember(d => d.SectorWorkedId, o => o.MapFrom(s => s.Info.SectorWorkedId))
            .ForMember(d => d.Performance, o => o.MapFrom(s => s.Info.Performance))
            .ForMember(d => d.WorkedHours, o => o.MapFrom(s => s.Info.WorkedHours))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Info.Description.ToUpper()))
            .ForMember(d => d.ActivityDate, o => o.MapFrom(s => (DateTime)s.ActivityDate))
            .ForMember(d => d.RowVersion, o => o.Ignore());

        #endregion
    }

    private static RecipeEntity MapRecipeToEntity(RecipeWorkOrderDTO recipeDTO, ResolutionContext context)
    {
        var groupedDetails = GroupItems(recipeDTO.Details ?? new List<DetailRecipeDTO>());

        return new RecipeEntity
        {
            VolumeMachine = recipeDTO.VolumeMachine,
            VolumeMachineUnit = recipeDTO.VolumeMachineUnit,
            MachineId = recipeDTO.MachineId,
            State = recipeDTO.Status,
            TRV = recipeDTO.TRV,
            IsDeleted = false,
            DetailRecipeList = groupedDetails.Select(dr => context.Mapper.Map<DetailRecipeEntity>(dr)).ToList()
        };
    }

    private static List<DetailRecipeDTO> GroupItems(List<DetailRecipeDTO> materialList)
    {
        return materialList
            .GroupBy(d => new { d.CategoryId, d.MaterialId, d.Brand, d.AmountRequiredUnit, d.EstimatedAmountUnit })
            .Select(g => new DetailRecipeDTO
            {
                CategoryId = g.Key.CategoryId,
                MaterialId = g.Key.MaterialId,
                Brand = g.Key.Brand,
                AmountRequiredUnit = g.Key.AmountRequiredUnit,
                EstimatedAmountUnit = g.Key.EstimatedAmountUnit,

                Material = g.First().Material,

                // Sumamos las cantidades
                AmountRequired = g.Sum(x => x.AmountRequired),
                EstimatedAmount = g.Sum(x => x.EstimatedAmount),
                TotalAmountConsumed = g.Sum(x => x.TotalAmountConsumed),

                PestDisease = string.Join(", ", g.Select(x => x.PestDisease).Distinct())
            })
            .ToList();
    }
}
