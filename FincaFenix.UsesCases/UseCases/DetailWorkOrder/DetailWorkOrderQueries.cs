using AutoMapper;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.DetailWorkOrder;

public record GetActivitiesByOrderIdQuery(int OrderId) : IRequest<IEnumerable<ActivityWorkOrderDTO>>;

public class GetActivitiesByOrderIdHandler(IGetActivitiesWorkOrderRepository repository, IMapper mapper) : IRequestHandler<GetActivitiesByOrderIdQuery, IEnumerable<ActivityWorkOrderDTO>>
{
    public async Task<IEnumerable<ActivityWorkOrderDTO>> Handle(GetActivitiesByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var details = await repository.GetActivityLogByOrderId(request.OrderId);
        return mapper.Map<List<ActivityWorkOrderDTO>>(details);
    }
}
