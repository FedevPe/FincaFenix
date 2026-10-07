using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.MaterialCategory;

public record GetMaterialCategoriesQuery : IRequest<IEnumerable<MaterialCategoryDTO>>;

public class GetMaterialCategoriesHandler(IMaterialCategoryRepository repository, IMapper mapper) : IRequestHandler<GetMaterialCategoriesQuery, IEnumerable<MaterialCategoryDTO>>
{
    public async Task<IEnumerable<MaterialCategoryDTO>> Handle(GetMaterialCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await repository.GetAllCategories();
        return mapper.Map<IEnumerable<MaterialCategoryDTO>>(categories);
    }
}
