using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Fruit;

public record GetFruitVarietiesQuery(int? FruitId) : IRequest<IEnumerable<FruitVarietyDTO>>;
public record GetFruitVarietiesPagedQuery(FruitVarietyFilterDTO Filter) : IRequest<PagedResult<FruitVarietyDTO>>;
public record GetFruitVarietyByIdQuery(int Id) : IRequest<FruitVarietyDTO>;

public class GetFruitVarietiesHandler(IFruitVarietyRepository repository, IMapper mapper) : IRequestHandler<GetFruitVarietiesQuery, IEnumerable<FruitVarietyDTO>>
{
    public async Task<IEnumerable<FruitVarietyDTO>> Handle(GetFruitVarietiesQuery request, CancellationToken cancellationToken)
    {
        var varieties = await repository.GetAll(request.FruitId);
        return mapper.Map<IEnumerable<FruitVarietyDTO>>(varieties);
    }
}

public class GetFruitVarietiesPagedHandler(IFruitVarietyRepository repository, IMapper mapper) : IRequestHandler<GetFruitVarietiesPagedQuery, PagedResult<FruitVarietyDTO>>
{
    public async Task<PagedResult<FruitVarietyDTO>> Handle(GetFruitVarietiesPagedQuery request, CancellationToken cancellationToken)
    {
        var (varieties, totalCount) = await repository.GetPaged(request.Filter);

        return new PagedResult<FruitVarietyDTO>
        {
            Items = mapper.Map<IEnumerable<FruitVarietyDTO>>(varieties),
            TotalCount = totalCount,
            Page = request.Filter.PageNumber,
            PageSize = request.Filter.PageSize
        };
    }
}

public class GetFruitVarietyByIdHandler(IFruitVarietyRepository repository, IMapper mapper) : IRequestHandler<GetFruitVarietyByIdQuery, FruitVarietyDTO>
{
    public async Task<FruitVarietyDTO> Handle(GetFruitVarietyByIdQuery request, CancellationToken cancellationToken)
    {
        var variety = await repository.GetById(request.Id);

        if (variety is null)
            throw new NotFoundException($"No se encontró la variedad con Id {request.Id}.");

        return mapper.Map<FruitVarietyDTO>(variety);
    }
}
