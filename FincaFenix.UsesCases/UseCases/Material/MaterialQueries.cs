using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Material;

public record GetMaterialListQuery : IRequest<IEnumerable<MaterialRecipeDTO>>;
public record GetMaterialListByCategoryQuery(int CategoryId) : IRequest<IEnumerable<MaterialRecipeDTO>>;
public record GetMaterialListByRecipeQuery(int RecipeId) : IRequest<IEnumerable<MaterialRecipeDTO>>;
public record GetMaterialByIdQuery(int Id) : IRequest<MaterialDTO>;
public record GetMaterialListPagedQuery(MaterialFilterDTO Filter) : IRequest<PagedResult<MaterialDTO>>;

public class GetMaterialListHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<GetMaterialListQuery, IEnumerable<MaterialRecipeDTO>>
{
    public async Task<IEnumerable<MaterialRecipeDTO>> Handle(GetMaterialListQuery request, CancellationToken cancellationToken)
    {
        var materials = await repository.GetMaterialList();
        return mapper.Map<IEnumerable<MaterialRecipeDTO>>(materials);
    }
}

public class GetMaterialListByCategoryHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<GetMaterialListByCategoryQuery, IEnumerable<MaterialRecipeDTO>>
{
    public async Task<IEnumerable<MaterialRecipeDTO>> Handle(GetMaterialListByCategoryQuery request, CancellationToken cancellationToken)
    {
        var materials = await repository.GetAllMaterialByCategoryId(request.CategoryId);
        return mapper.Map<IEnumerable<MaterialRecipeDTO>>(materials);
    }
}

public class GetMaterialListByRecipeHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<GetMaterialListByRecipeQuery, IEnumerable<MaterialRecipeDTO>>
{
    public async Task<IEnumerable<MaterialRecipeDTO>> Handle(GetMaterialListByRecipeQuery request, CancellationToken cancellationToken)
    {
        var materials = await repository.GetAllMaterialByRecipeId(request.RecipeId);
        return mapper.Map<IEnumerable<MaterialRecipeDTO>>(materials);
    }
}

public class GetMaterialByIdHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<GetMaterialByIdQuery, MaterialDTO>
{
    public async Task<MaterialDTO> Handle(GetMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        var material = await repository.GetMaterialById(request.Id);

        if (material is null)
            throw new NotFoundException($"No se encontró el material con Id {request.Id}.");

        return mapper.Map<MaterialDTO>(material);
    }
}

public class GetMaterialListPagedHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<GetMaterialListPagedQuery, PagedResult<MaterialDTO>>
{
    public async Task<PagedResult<MaterialDTO>> Handle(GetMaterialListPagedQuery request, CancellationToken cancellationToken)
    {
        var (materials, totalCount) = await repository.GetMaterialListPaged(request.Filter);

        return new PagedResult<MaterialDTO>
        {
            Items = mapper.Map<IEnumerable<MaterialDTO>>(materials),
            TotalCount = totalCount,
            Page = request.Filter.PageNumber,
            PageSize = request.Filter.PageSize
        };
    }
}
