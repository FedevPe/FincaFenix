using AutoMapper;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.Enum;
using FincaFenix.Entities.Rendimiento;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.DetailWorkOrder;

public record GetActivitiesByOrderIdQuery(int OrderId) : IRequest<IEnumerable<ActivityWorkOrderDTO>>;

public class GetActivitiesByOrderIdHandler(IGetActivitiesWorkOrderRepository repository, IMapper mapper) : IRequestHandler<GetActivitiesByOrderIdQuery, IEnumerable<ActivityWorkOrderDTO>>
{
    public async Task<IEnumerable<ActivityWorkOrderDTO>> Handle(GetActivitiesByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var details = (await repository.GetActivityLogByOrderId(request.OrderId)).ToList();
        var dtos = mapper.Map<List<ActivityWorkOrderDTO>>(details);

        for (var i = 0; i < dtos.Count; i++)
        {
            var mode = details[i].WorkOrder?.Task?.RendimientoMode ?? RendimientoModeEnum.ManHours;
            dtos[i].RendimientoMode = mode.ToString();

            switch (mode)
            {
                case RendimientoModeEnum.AreaPerManHour:
                    dtos[i].Rendimiento = RendimientoCalculator.PerManHour(dtos[i].AreaWorked, dtos[i].WorkedHours);
                    dtos[i].RendimientoUnit = "ha/h";
                    break;
                case RendimientoModeEnum.OutputPerManHour:
                    dtos[i].Rendimiento = RendimientoCalculator.PerManHour(dtos[i].ProducedAmount, dtos[i].WorkedHours);
                    dtos[i].RendimientoUnit = "kg/h";
                    break;
                case RendimientoModeEnum.MaterialEfficiency:
                    dtos[i].Rendimiento = null;
                    dtos[i].RendimientoUnit = "%";
                    break;
                default:
                    dtos[i].Rendimiento = RendimientoCalculator.ManHours(dtos[i].WorkedHours);
                    dtos[i].RendimientoUnit = "h";
                    break;
            }
        }

        return dtos;
    }
}
