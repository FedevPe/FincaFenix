using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.DetailSector;

public record GetSectorListByFarmQuery(int FarmId) : IRequest<IEnumerable<DetailSectorFarmDTO>>;
public record GetSectorListByOrderQuery(int OrderId) : IRequest<IEnumerable<DetailSectorFarmDTO>>;

public class GetSectorListByFarmHandler(IDetailSectorRepository repository, IMapper mapper) : IRequestHandler<GetSectorListByFarmQuery, IEnumerable<DetailSectorFarmDTO>>
{
    public async Task<IEnumerable<DetailSectorFarmDTO>> Handle(GetSectorListByFarmQuery request, CancellationToken cancellationToken)
    {
        var sectors = await repository.GetAllSectorsByFarmId(request.FarmId);
        return mapper.Map<IEnumerable<DetailSectorFarmDTO>>(sectors);
    }
}

public class GetSectorListByOrderHandler(IDetailSectorRepository repository, IMapper mapper) : IRequestHandler<GetSectorListByOrderQuery, IEnumerable<DetailSectorFarmDTO>>
{
    public async Task<IEnumerable<DetailSectorFarmDTO>> Handle(GetSectorListByOrderQuery request, CancellationToken cancellationToken)
    {
        var sectors = await repository.GetAllSectorsByOrderId(request.OrderId);
        return mapper.Map<IEnumerable<DetailSectorFarmDTO>>(sectors);
    }
}
