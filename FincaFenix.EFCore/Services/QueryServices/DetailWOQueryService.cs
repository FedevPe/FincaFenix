using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class DetailWOQueryService : FincaFenixContext, IDetailWOQueryService
    {
        public async Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId)
        {
            var listDetails = await DetailWorkOrders
                .Where(dwo => dwo.WorkOrderId == orderId)
                .Include(dwo => dwo.SectorWorked)
                .Include(dwo => dwo.Employee)
                .OrderBy(dwo => dwo.ActivityDate)
                .ToListAsync();

            return listDetails;
        }
    }
}
