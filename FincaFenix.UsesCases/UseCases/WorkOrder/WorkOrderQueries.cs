using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Aggregates;
using FincaFenix.UsesCases.Repository.WorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.WorkOrder;

public record GetAllWorkOrdersQuery : IRequest<List<ShowWorkOrderDTO>>;
public record GetWorkOrderByIdQuery(int Id) : IRequest<ShowWorkOrderDTO>;
public record GetWorkOrderInfoByIdQuery(int Id) : IRequest<InfoWorkOrderDTO>;
public record GetWorkOrderListPaginatedQuery(int PageNumber, int PageSize, string Status) : IRequest<PagedResult<ShowWorkOrderDTO>>;

public class GetAllWorkOrdersHandler(IGetWorkOrderInformationRepository repository) : IRequestHandler<GetAllWorkOrdersQuery, List<ShowWorkOrderDTO>>
{
    public async Task<List<ShowWorkOrderDTO>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllWorkOrderList();
        return entities.Select(WorkOrderMapper.EntityToDTO).ToList();
    }
}

public class GetWorkOrderByIdHandler(IGetWorkOrderInformationRepository repository) : IRequestHandler<GetWorkOrderByIdQuery, ShowWorkOrderDTO>
{
    public async Task<ShowWorkOrderDTO> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetWorkOrderAndRecipeByIdWorkorder(request.Id);
        return WorkOrderMapper.EntityToDTO(entity);
    }
}

public class GetWorkOrderInfoByIdHandler(IGetWorkOrderInformationRepository repository) : IRequestHandler<GetWorkOrderInfoByIdQuery, InfoWorkOrderDTO>
{
    public async Task<InfoWorkOrderDTO> Handle(GetWorkOrderInfoByIdQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetWorkOrderById(request.Id);
    }
}

public class GetWorkOrderListPaginatedHandler(IGetWorkOrderInformationRepository repository) : IRequestHandler<GetWorkOrderListPaginatedQuery, PagedResult<ShowWorkOrderDTO>>
{
    public async Task<PagedResult<ShowWorkOrderDTO>> Handle(GetWorkOrderListPaginatedQuery request, CancellationToken cancellationToken)
    {
        var (workOrders, totalCount) = await repository.GetWorkOrderList(request.PageNumber, request.PageSize, request.Status);

        return new PagedResult<ShowWorkOrderDTO>
        {
            Items = workOrders,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
