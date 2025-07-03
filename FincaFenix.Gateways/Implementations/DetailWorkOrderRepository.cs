using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class DetailWorkOrderRepository(
        IDetailWOCommandService command) : IDetailWorkOrderRepository
    {
        public async Task<int> CreateDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            return await command.SaveDetailWorkOrderAsync(detailWorkOrder);
        }
    }
}
