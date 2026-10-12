using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices
{
    public class DetailWOCommandService(
        FincaFenixContext context) : IDetailWOCommandService
    {
        public async Task<int> SaveDetailWorkOrderAsync(DetailWorkOrderEntity detailWorkOrder)
        {
            await context.DetailWorkOrders.AddAsync(detailWorkOrder);
            await context.SaveChangesAsync();
            return detailWorkOrder.Id;
        }

        public async Task<decimal> GetAccumulatedAreaPercentageAsync(int workOrderId, int sectorId, bool forUpdate = false)
        {
            var hint = forUpdate ? "WITH (UPDLOCK, HOLDLOCK) " : string.Empty;

            var sql = $@"
                SELECT ISNULL(SUM([PorcentajeAreaTrabajada]), 0) AS Value
                FROM [DetalleOrdenTrabajo] {hint}
                WHERE [IdOrdenTrabajo] = {{0}} AND [IdSectorTrabajado] = {{1}}";

            return await context.Database
                .SqlQueryRaw<decimal>(sql, workOrderId, sectorId)
                .FirstAsync();
        }
    }
}
