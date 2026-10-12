using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices
{
    public interface IDetailWOCommandService
    {
        Task<int> SaveDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder);

        Task<decimal> GetAccumulatedAreaPercentageAsync(int workOrderId, int sectorId, bool forUpdate = false);
    }
}
