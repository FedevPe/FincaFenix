using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;

namespace FincaFenix.Gateways.Implementations.DetailWorkOrder
{
    public class GetActivitiesWorkOrderRepository(
        IDetailWOQueryService query) : IGetActivitiesWorkOrderRepository
    {
        public async Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId)
        {
            return await query.GetActivityLogByOrderId(orderId);
        }
    }
}
