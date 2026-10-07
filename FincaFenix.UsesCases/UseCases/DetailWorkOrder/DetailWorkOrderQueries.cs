using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.DetailWorkOrder;

public record GetActivitiesByOrderIdQuery(int OrderId) : IRequest<IEnumerable<ActivityWorkOrderDTO>>;

public class GetActivitiesByOrderIdHandler(IGetActivitiesWorkOrderRepository repository) : IRequestHandler<GetActivitiesByOrderIdQuery, IEnumerable<ActivityWorkOrderDTO>>
{
    public async Task<IEnumerable<ActivityWorkOrderDTO>> Handle(GetActivitiesByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var details = await repository.GetActivityLogByOrderId(request.OrderId);
        return details.Select(d => new ActivityWorkOrderDTO
        {
            Id = d.Id,
            Performance = d.Performance,
            WorkedHours = d.WorkedHours,
            Description = d.Description.ToUpper(),
            ActivityDate = d.ActivityDate,
            Employee = d.Employee != null ? new EmployeeDTO
            {
                Id = d.Employee.Id,
                Name = d.Employee.Name,
                LastName = d.Employee.LastName
            } : null,
            Sector = d.SectorWorked != null ? new DetailSectorFarmDTO
            {
                Id = d.SectorWorked.Id,
                SectorName = d.SectorWorked.SectorName
            } : null
        }).ToList();
    }
}
