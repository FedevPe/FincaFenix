using AutoMapper;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Farm;

public record GetFarmListQuery : IRequest<IEnumerable<FarmDTO>>;
public record GetFarmByIdQuery(int Id) : IRequest<FarmDTO>;

public class GetFarmListHandler(IFarmRepository repository, IMapper mapper) : IRequestHandler<GetFarmListQuery, IEnumerable<FarmDTO>>
{
    public async Task<IEnumerable<FarmDTO>> Handle(GetFarmListQuery request, CancellationToken cancellationToken)
    {
        var farms = await repository.GetListFarm();
        return mapper.Map<IEnumerable<FarmDTO>>(farms);
    }
}

public class GetFarmByIdHandler(IFarmRepository repository, IMapper mapper) : IRequestHandler<GetFarmByIdQuery, FarmDTO>
{
    public async Task<FarmDTO> Handle(GetFarmByIdQuery request, CancellationToken cancellationToken)
    {
        var farm = await repository.GetFarmById(request.Id);
        return mapper.Map<FarmDTO>(farm);
    }
}
