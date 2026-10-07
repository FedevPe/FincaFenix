using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Material;

public record GetMaterialListQuery : IRequest<IEnumerable<MaterialRecipeDTO>>;
public record GetMaterialListByCategoryQuery(int CategoryId) : IRequest<IEnumerable<MaterialRecipeDTO>>;
public record GetMaterialListByRecipeQuery(int RecipeId) : IRequest<IEnumerable<MaterialRecipeDTO>>;

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

public class GetMaterialListByRecipeHandler : IRequestHandler<GetMaterialListByRecipeQuery, IEnumerable<MaterialRecipeDTO>>
{
    public Task<IEnumerable<MaterialRecipeDTO>> Handle(GetMaterialListByRecipeQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
