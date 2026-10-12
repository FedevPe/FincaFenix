using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Fruit;

public record GetFruitsQuery : IRequest<IEnumerable<FruitDTO>>;
public record GetFruitsPagedQuery(FruitFilterDTO Filter) : IRequest<PagedResult<FruitDTO>>;
public record GetFruitByIdQuery(int Id) : IRequest<FruitDTO>;

public class GetFruitsHandler(IFruitRepository repository, IMapper mapper) : IRequestHandler<GetFruitsQuery, IEnumerable<FruitDTO>>
{
    public async Task<IEnumerable<FruitDTO>> Handle(GetFruitsQuery request, CancellationToken cancellationToken)
    {
        var fruits = await repository.GetAll();
        return mapper.Map<IEnumerable<FruitDTO>>(fruits);
    }
}

public class GetFruitsPagedHandler(IFruitRepository repository, IMapper mapper) : IRequestHandler<GetFruitsPagedQuery, PagedResult<FruitDTO>>
{
    public async Task<PagedResult<FruitDTO>> Handle(GetFruitsPagedQuery request, CancellationToken cancellationToken)
    {
        var (fruits, totalCount) = await repository.GetPaged(request.Filter);

        return new PagedResult<FruitDTO>
        {
            Items = mapper.Map<IEnumerable<FruitDTO>>(fruits),
            TotalCount = totalCount,
            Page = request.Filter.PageNumber,
            PageSize = request.Filter.PageSize
        };
    }
}

public class GetFruitByIdHandler(IFruitRepository repository, IMapper mapper) : IRequestHandler<GetFruitByIdQuery, FruitDTO>
{
    public async Task<FruitDTO> Handle(GetFruitByIdQuery request, CancellationToken cancellationToken)
    {
        var fruit = await repository.GetById(request.Id);

        if (fruit is null)
            throw new NotFoundException($"No se encontró la fruta con Id {request.Id}.");

        return mapper.Map<FruitDTO>(fruit);
    }
}
