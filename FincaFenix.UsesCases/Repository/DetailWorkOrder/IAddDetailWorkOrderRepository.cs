using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.DetailWorkOrder
{
    public interface IAddDetailWorkOrderRepository
    {
        Task<int> CreateDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder);
    }
}
