using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;

namespace FincaFenix.Presenters.Implementations.DetailWorkOrder
{
    public class GetActivitiesWorkOrderPresenter : IGetActivitiesWorkOrderOutputPort
    {
        public IEnumerable<ActivityWorkOrderDTO>? ActivityLog {  get; private set; }

        public async Task HandleList(IEnumerable<DetailWorkOrderEntity> entities)
        {
            ActivityLog = entities.Select(e => new ActivityWorkOrderDTO
            {
                Id = e.Id,
                Performance = e.Performance,
                WorkedHours = e.WorkedHours,
                Description = e.Description.ToUpper(),
                ActivityDate = e.ActivityDate,
                WorkOrder = null,
                Employee = e.Employee != null ? new EmployeeDTO
                {
                    Id = e.Employee.Id,
                    Name = e.Employee.Name,
                    LastName = e.Employee.LastName
                } : null,
                Sector = e.SectorWorked != null ? new DetailSectorFarmDTO
                {
                    Id = e.SectorWorked.Id,
                    SectorName = e.SectorWorked.SectorName,
                } : null

            }).ToList();
            await Task.CompletedTask;
        }
    }
}
