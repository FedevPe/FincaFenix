using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.MaterialCategory;

public record GetMaterialCategoriesQuery : IRequest<IEnumerable<MaterialCategoryDTO>>;
public record GetMaterialCategoryByIdQuery(int Id) : IRequest<MaterialCategoryDTO>;

public class GetMaterialCategoriesHandler(IMaterialCategoryRepository repository, IMapper mapper) : IRequestHandler<GetMaterialCategoriesQuery, IEnumerable<MaterialCategoryDTO>>
{
    public async Task<IEnumerable<MaterialCategoryDTO>> Handle(GetMaterialCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAllCategories();
        return mapper.Map<IEnumerable<MaterialCategoryDTO>>(categories);
    }
}

public class GetMaterialCategoryByIdHandler(IMaterialCategoryRepository repository, IMapper mapper) : IRequestHandler<GetMaterialCategoryByIdQuery, MaterialCategoryDTO>
{
    public async Task<MaterialCategoryDTO> Handle(GetMaterialCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetById(request.Id);

        if (category is null)
            throw new NotFoundException($"No se encontró la categoría de material con Id {request.Id}.");

        return mapper.Map<MaterialCategoryDTO>(category);
    }
}
