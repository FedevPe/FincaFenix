using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;

namespace FincaFenix.EFCore.Services.CommandServices
{
    public class DetailWOCommandService(
        FincaFenixContext context) : IDetailWOCommandService
    {
        public async Task<int> SaveDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            try
            {
                await context.DetailWorkOrders.AddAsync(detailWorkOrder);
                await context.SaveChangesAsync();
                return detailWorkOrder.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
