using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;

namespace FincaFenix.Gateways.Implementations.DetailWorkOrder
{
    public class AddDetailWorkOrderRepository (
        IDetailWOCommandService command) : IAddDetailWorkOrderRepository
    {
        public async Task<int> CreateDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            return await command.SaveDetailWorkOrderAsync(detailWorkOrder);
        }
    }
}
