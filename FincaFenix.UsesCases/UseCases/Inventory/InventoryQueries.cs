using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.CostDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.InventoryDTOs.ReservationDTOs;
using FincaFenix.UsesCases.Repository.Inventory;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Inventory;

public record GetStockByFarmQuery(int FarmId) : IRequest<IEnumerable<StockDTO>>;
public record GetConsolidatedStockQuery : IRequest<IEnumerable<StockDTO>>;
public record GetLowStockQuery : IRequest<IEnumerable<StockDTO>>;
public record GetZeroStockQuery : IRequest<IEnumerable<StockDTO>>;
public record GetCurrenciesQuery : IRequest<IEnumerable<CurrencyDTO>>;
public record GetCurrentMaterialCostsQuery : IRequest<IEnumerable<CurrentMaterialCostDTO>>;
public record GetCurrentMaterialCostsPagedQuery(MaterialFilterDTO Filter) : IRequest<PagedResult<CurrentMaterialCostDTO>>;
public record GetMaterialReservationsQuery(int MaterialId) : IRequest<MaterialReservationsDTO>;
public record GetMaterialCostHistoryQuery(int MaterialId) : IRequest<CostHistoryDTO>;
public record GetWorkOrderCostsQuery(int WorkOrderId) : IRequest<WorkOrderCostDTO>;
public record GetRecentMovementsQuery(int Take) : IRequest<IEnumerable<InventoryMovementDTO>>;

public class GetStockByFarmHandler(IInventoryRepository repository) : IRequestHandler<GetStockByFarmQuery, IEnumerable<StockDTO>>
{
    public Task<IEnumerable<StockDTO>> Handle(GetStockByFarmQuery request, CancellationToken cancellationToken)
        => repository.GetStockByFarmAsync(request.FarmId);
}

public class GetConsolidatedStockHandler(IInventoryRepository repository) : IRequestHandler<GetConsolidatedStockQuery, IEnumerable<StockDTO>>
{
    public Task<IEnumerable<StockDTO>> Handle(GetConsolidatedStockQuery request, CancellationToken cancellationToken)
        => repository.GetConsolidatedStockAsync();
}

public class GetLowStockHandler(IInventoryRepository repository) : IRequestHandler<GetLowStockQuery, IEnumerable<StockDTO>>
{
    public Task<IEnumerable<StockDTO>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
        => repository.GetLowStockAsync();
}

public class GetZeroStockHandler(IInventoryRepository repository) : IRequestHandler<GetZeroStockQuery, IEnumerable<StockDTO>>
{
    public Task<IEnumerable<StockDTO>> Handle(GetZeroStockQuery request, CancellationToken cancellationToken)
        => repository.GetZeroStockAsync();
}

public class GetCurrenciesHandler(IInventoryRepository repository) : IRequestHandler<GetCurrenciesQuery, IEnumerable<CurrencyDTO>>
{
    public Task<IEnumerable<CurrencyDTO>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
        => repository.GetCurrenciesAsync();
}

public class GetCurrentMaterialCostsHandler(IInventoryRepository repository) : IRequestHandler<GetCurrentMaterialCostsQuery, IEnumerable<CurrentMaterialCostDTO>>
{
    public Task<IEnumerable<CurrentMaterialCostDTO>> Handle(GetCurrentMaterialCostsQuery request, CancellationToken cancellationToken)
        => repository.GetCurrentMaterialCostsAsync();
}

public class GetMaterialCostHistoryHandler(IInventoryRepository repository) : IRequestHandler<GetMaterialCostHistoryQuery, CostHistoryDTO>
{
    public Task<CostHistoryDTO> Handle(GetMaterialCostHistoryQuery request, CancellationToken cancellationToken)
        => repository.GetMaterialCostHistoryAsync(request.MaterialId);
}

public class GetWorkOrderCostsHandler(IInventoryRepository repository) : IRequestHandler<GetWorkOrderCostsQuery, WorkOrderCostDTO>
{
    public Task<WorkOrderCostDTO> Handle(GetWorkOrderCostsQuery request, CancellationToken cancellationToken)
        => repository.GetWorkOrderCostsAsync(request.WorkOrderId);
}

public class GetCurrentMaterialCostsPagedHandler(IInventoryRepository repository) : IRequestHandler<GetCurrentMaterialCostsPagedQuery, PagedResult<CurrentMaterialCostDTO>>
{
    public Task<PagedResult<CurrentMaterialCostDTO>> Handle(GetCurrentMaterialCostsPagedQuery request, CancellationToken cancellationToken)
        => repository.GetCurrentMaterialCostsPagedAsync(request.Filter);
}

public class GetMaterialReservationsHandler(IInventoryRepository repository) : IRequestHandler<GetMaterialReservationsQuery, MaterialReservationsDTO>
{
    public Task<MaterialReservationsDTO> Handle(GetMaterialReservationsQuery request, CancellationToken cancellationToken)
        => repository.GetMaterialReservationsAsync(request.MaterialId);
}

public class GetRecentMovementsHandler(IInventoryRepository repository) : IRequestHandler<GetRecentMovementsQuery, IEnumerable<InventoryMovementDTO>>
{
    public Task<IEnumerable<InventoryMovementDTO>> Handle(GetRecentMovementsQuery request, CancellationToken cancellationToken)
        => repository.GetRecentMovementsAsync(request.Take);
}
