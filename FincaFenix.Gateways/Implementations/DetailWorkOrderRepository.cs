using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class DetailWorkOrderRepository(
        IDetailWOCommandService command,
        IDetailWOQueryService query) : IDetailWorkOrderRepository
    {
        public async Task<int> CreateDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            return await command.SaveDetailWorkOrderAsync(detailWorkOrder);
        }
        public async Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId)
        {
            return await query.GetActivityLogByOrderId(orderId);
        }
    }
}
