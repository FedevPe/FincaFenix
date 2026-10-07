using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.UsesCases.Mappers;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.DetailWorkOrder;

public record AddDetailWorkOrderCommand(AddDetailWorkOrderDTO Dto) : IRequest<bool>;

public class AddDetailWorkOrderHandler(IAddDetailWorkOrderRepository repository) : IRequestHandler<AddDetailWorkOrderCommand, bool>
{
    public async Task<bool> Handle(AddDetailWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = DetailWorkOrderMapper.ToEntity(request.Dto);
        var id = await repository.CreateDetailWorkOrderAsync(entity);
        return id > 0;
    }
}
