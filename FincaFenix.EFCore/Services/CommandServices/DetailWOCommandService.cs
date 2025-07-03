using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;

namespace FincaFenix.EFCore.Services.CommandServices
{
    public class DetailWOCommandService : FincaFenixContext, IDetailWOCommandService
    {
        public async Task<int> SaveDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            try
            {
                await DetailWorkOrders.AddAsync(detailWorkOrder);
                await SaveChangesAsync();
                return detailWorkOrder.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
