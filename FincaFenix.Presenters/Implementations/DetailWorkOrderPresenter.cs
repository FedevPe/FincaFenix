using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenix.Presenters.Implementations
{
    public class DetailWorkOrderPresenter : IDetailWorkOrderOutputPort
    {
        public bool IsSuccess { get; private set; } = false;
        public IEnumerable<DetailWorkOrderDTO>? ActivityLog {  get; private set; }
        public async Task Handle(int detailWOId)
        {
            if (detailWOId > 0)
            {
                IsSuccess = true;
            }
            await Task.CompletedTask;
        }

        public async Task HandleList(IEnumerable<DetailWorkOrderEntity> entities)
        {
            ActivityLog = entities.Select(e => new DetailWorkOrderDTO
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
                    Id= e.SectorWorked.Id,
                    SectorName = e.SectorWorked.SectorName,
                } : null

            }).ToList();
            await Task.CompletedTask;
        }
    }
}
