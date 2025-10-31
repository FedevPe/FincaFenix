using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.DetailWorkOrder
{
    public interface IGetActivitiesWorkOrderRepository
    {
        Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId);
    }
}
