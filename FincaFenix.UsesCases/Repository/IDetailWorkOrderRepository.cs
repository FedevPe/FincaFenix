using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IDetailWorkOrderRepository
    {
        Task<int> CreateDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder);
        Task<IEnumerable<DetailWorkOrderEntity>> GetActivityLogByOrderId(int orderId);
    }
}
