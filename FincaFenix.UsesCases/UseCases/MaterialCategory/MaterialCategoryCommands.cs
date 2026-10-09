using AutoMapper;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.MaterialCategory;

public record CreateMaterialCategoryCommand(SaveMaterialCategoryDTO Dto) : IRequest<MaterialCategoryDTO>;
public record UpdateMaterialCategoryCommand(SaveMaterialCategoryDTO Dto) : IRequest<MaterialCategoryDTO>;
public record DeleteMaterialCategoryCommand(int Id) : IRequest<Unit>;

public class CreateMaterialCategoryHandler(IMaterialCategoryRepository repository, IMapper mapper) : IRequestHandler<CreateMaterialCategoryCommand, MaterialCategoryDTO>
{
    public async Task<MaterialCategoryDTO> Handle(CreateMaterialCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<MaterialCategoryEntity>(request.Dto);
        var created = await repository.Add(entity);
        return mapper.Map<MaterialCategoryDTO>(created);
    }
}

public class UpdateMaterialCategoryHandler(IMaterialCategoryRepository repository, IMapper mapper) : IRequestHandler<UpdateMaterialCategoryCommand, MaterialCategoryDTO>
{
    public async Task<MaterialCategoryDTO> Handle(UpdateMaterialCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<MaterialCategoryEntity>(request.Dto);
        var updated = await repository.Update(entity);
        return mapper.Map<MaterialCategoryDTO>(updated);
    }
}

public class DeleteMaterialCategoryHandler(IMaterialCategoryRepository repository) : IRequestHandler<DeleteMaterialCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMaterialCategoryCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}
