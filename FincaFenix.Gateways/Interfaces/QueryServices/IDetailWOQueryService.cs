using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IDetailWOQueryService
    {
        Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId);
    }
}
