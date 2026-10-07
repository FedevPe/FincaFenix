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
                    Performance = dwo.Performance,
                    WorkedHours = dwo.WorkedHours,
                    Description = dwo.Description,
                    ActivityDate = dwo.ActivityDate,
                    Employee = dwo.Employee != null ? new EmployeeEntity
                    {
                        Id = dwo.Employee.Id,
                        Name = dwo.Employee.Name,
                        LastName = dwo.Employee.LastName
                    } : null,
                    SectorWorked = dwo.SectorWorked != null ? new DetailSectorFarmEntity
                    {
                        Id = dwo.SectorWorked.Id,
                        SectorName = dwo.SectorWorked.SectorName
                    } : null
                })
                .OrderBy(dwo => dwo.ActivityDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
