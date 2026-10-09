using AutoMapper;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Material;

public record CreateMaterialCommand(CreateMaterialDTO Dto) : IRequest<MaterialDTO>;
public record UpdateMaterialCommand(UpdateMaterialDTO Dto) : IRequest<MaterialDTO>;
public record DeleteMaterialCommand(int Id) : IRequest<Unit>;

public class CreateMaterialHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<CreateMaterialCommand, MaterialDTO>
{
    public async Task<MaterialDTO> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<MaterialEntity>(request.Dto);
        var created = await repository.CreateMaterial(entity);
        var full = await repository.GetMaterialById(created.Id);
        return mapper.Map<MaterialDTO>(full ?? created);
    }
}

public class UpdateMaterialHandler(IMaterialRepository repository, IMapper mapper) : IRequestHandler<UpdateMaterialCommand, MaterialDTO>
{
    public async Task<MaterialDTO> Handle(UpdateMaterialCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<MaterialEntity>(request.Dto);
        var updated = await repository.UpdateMaterial(entity);
        var full = await repository.GetMaterialById(updated.Id);
        return mapper.Map<MaterialDTO>(full ?? updated);
    }
}

public class DeleteMaterialHandler(IMaterialRepository repository) : IRequestHandler<DeleteMaterialCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMaterialCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteMaterial(request.Id);
        return Unit.Value;
    }
}
