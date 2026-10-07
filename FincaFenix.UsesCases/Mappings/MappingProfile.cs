using AutoMapper;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.RecipeDTO;
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
            .ForMember(d => d.DiseasePlague, o => o.Ignore());

        CreateMap<DetailSectorFarmEntity, DetailSectorFarmDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.FarmId, o => o.MapFrom(s => s.FarmId))
            .ForMember(d => d.SectorName, o => o.MapFrom(s => s.SectorName))
            .ForMember(d => d.VarietyName, o => o.MapFrom(s => s.Variety != null ? s.Variety.Description : null))
            .ForMember(d => d.FruitName, o => o.MapFrom(s => s.Variety != null && s.Variety.Fruit != null ? s.Variety.Fruit.Description : null))
            .ForMember(d => d.NumberPlants, o => o.MapFrom(s => s.NumberPlants))
            .ForMember(d => d.Area, o => o.MapFrom(s => s.Area))
             .ForMember(d => d.Selected, o => o.Ignore());
;

        CreateMap<EmployeeEntity, EmployeeDTO>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));
    }
}