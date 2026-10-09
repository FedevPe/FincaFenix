using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class DetailWOQueryService (
        FincaFenixContext context) : IDetailWOQueryService
    {
        public async Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId)
        {
            return await context.DetailWorkOrders
                .Where(dwo => dwo.WorkOrderId == orderId)
                .Select(dwo => new DetailWorkOrderEntity
                {
                    Id = dwo.Id,
                    WorkOrderId = dwo.WorkOrderId,
                    EmployeeId = dwo.EmployeeId,
                    SectorWorkedId = dwo.SectorWorkedId,
                    MachinePasses = dwo.MachinePasses,
                    WorkedHours = dwo.WorkedHours,
                    ProducedAmount = dwo.ProducedAmount,
                    Description = dwo.Description,
                    ActivityDate = dwo.ActivityDate,
                    WorkOrder = new WorkOrderEntity
                    {
                        Id = dwo.WorkOrderId,
                        Task = dwo.WorkOrder.Task != null ? new TaskEntity
                        {
                            Id = dwo.WorkOrder.Task.Id,
                            RendimientoMode = dwo.WorkOrder.Task.RendimientoMode
                        } : null
                    },
                    Employee = dwo.Employee != null ? new EmployeeEntity
                    {
                        Id = dwo.Employee.Id,
                        Name = dwo.Employee.Name,
                        LastName = dwo.Employee.LastName
                    } : null,
                    SectorWorked = dwo.SectorWorked != null ? new DetailSectorFarmEntity
                    {
                        Id = dwo.SectorWorked.Id,
                        SectorName = dwo.SectorWorked.SectorName,
                        Area = dwo.SectorWorked.Area
                    } : null
                })
                .OrderBy(dwo => dwo.ActivityDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
