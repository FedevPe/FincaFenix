using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.Rendimiento;
using FincaFenix.Entities.Units;
using FincaFenix.UsesCases.Repository.WorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.WorkOrder;

public record GetAllWorkOrdersQuery : IRequest<List<ShowWorkOrderDTO>>;
public record GetWorkOrderByIdQuery(int Id) : IRequest<ShowWorkOrderDTO>;
public record GetWorkOrderInfoByIdQuery(int Id) : IRequest<InfoWorkOrderDTO>;
public record GetWorkOrderListPaginatedQuery(int PageNumber, int PageSize, string Status) : IRequest<PagedResult<ShowWorkOrderDTO>>;

public class GetAllWorkOrdersHandler(IGetWorkOrderInformationRepository repository, IMapper mapper) : IRequestHandler<GetAllWorkOrdersQuery, List<ShowWorkOrderDTO>>
{
    public async Task<List<ShowWorkOrderDTO>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var entities = await repository.GetAllWorkOrderList();
        return mapper.Map<List<ShowWorkOrderDTO>>(entities);
    }
}

public class GetWorkOrderByIdHandler(IGetWorkOrderInformationRepository repository, IMapper mapper) : IRequestHandler<GetWorkOrderByIdQuery, ShowWorkOrderDTO>
{
    public async Task<ShowWorkOrderDTO> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetWorkOrderAndRecipeByIdWorkorder(request.Id);
        var dto = mapper.Map<ShowWorkOrderDTO>(entity);

        var mode = entity?.Task?.RendimientoMode ?? RendimientoModeEnum.ManHours;
        dto.RendimientoMode = mode.ToString();

        var areaTotal = entity?.WorkedSectors?.Sum(ws => ws.SectorFarm?.Area ?? 0m) ?? 0m;
        var manHours = entity?.DetailWorkOrderList?.Sum(d => d.WorkedHours) ?? 0m;
        var realMachinePasses = entity?.DetailWorkOrderList?.Sum(d => d.MachinePasses) ?? 0m;
        var producedTotal = entity?.DetailWorkOrderList?.Sum(d => d.ProducedAmount ?? 0m) ?? 0m;

        dto.TotalManHours = manHours;
        dto.TotalProducedAmount = producedTotal > 0 ? producedTotal : null;

        switch (mode)
        {
            case RendimientoModeEnum.MaterialEfficiency:
                dto.RendimientoUnit = "%";
                if (dto.Recipe is not null)
                {
                    var theoretical = RendimientoCalculator.TheoreticalMachinePasses(areaTotal, dto.Recipe.TRV, dto.Recipe.VolumeMachine);
                    dto.Recipe.TheoreticalMachinePasses = theoretical;
                    dto.Recipe.RealMachinePasses = realMachinePasses;
                    dto.Recipe.TheoreticalVolume = areaTotal * dto.Recipe.TRV;
                    dto.Recipe.RealVolume = realMachinePasses * dto.Recipe.VolumeMachine;
                    dto.Rendimiento = RendimientoCalculator.MaterialEfficiency(areaTotal, dto.Recipe.TRV, dto.Recipe.VolumeMachine, realMachinePasses);
                    dto.Recipe.Rendimiento = dto.Rendimiento;
                }
                break;
            case RendimientoModeEnum.AreaPerManHour:
                dto.RendimientoUnit = "ha/h";
                dto.Rendimiento = RendimientoCalculator.PerManHour(areaTotal, manHours);
                break;
            case RendimientoModeEnum.OutputPerManHour:
                dto.RendimientoUnit = "kg/h";
                dto.Rendimiento = RendimientoCalculator.PerManHour(producedTotal, manHours);
                break;
            default:
                dto.RendimientoUnit = "h";
                dto.Rendimiento = RendimientoCalculator.ManHours(manHours);
                break;
        }

        if (dto.Recipe?.Details is { Count: > 0 })
        {
            var consumedAmounts = await repository.GetConsumedAmountsByWorkOrderAsync(request.Id);

            foreach (var detail in dto.Recipe.Details)
            {
                if (consumedAmounts.TryGetValue(detail.MaterialId, out var consumed))
                {
                    detail.TotalAmountConsumed = consumed;
                }

                if (mode == RendimientoModeEnum.MaterialEfficiency)
                {
                    var materialUnitId = entity?.Recipe?.DetailRecipeList?
                        .FirstOrDefault(dr => dr.MaterialId == detail.MaterialId)?.Material?.UnitOfMeasureId;

                    var theoreticalRaw = (dto.Recipe.TheoreticalMachinePasses ?? 0m) * detail.AmountRequired;
                    decimal? theoreticalBase;

                    try
                    {
                        theoreticalBase = UnitConverter.ConvertToBaseUnit(theoreticalRaw, detail.AmountRequiredUnit, materialUnitId);
                    }
                    catch (BusinessRuleException)
                    {
                        theoreticalBase = theoreticalRaw;
                    }

                    detail.TheoreticalAmount = theoreticalBase;
                    detail.Rendimiento = RendimientoCalculator.EfficiencyFromAmounts(theoreticalBase, detail.TotalAmountConsumed);
                }
            }
        }

        return dto;
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
