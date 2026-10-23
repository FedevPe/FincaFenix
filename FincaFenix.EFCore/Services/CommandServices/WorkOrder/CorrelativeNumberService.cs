using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class CorrelativeNumberService(FincaFenixContext context) : ICorrelativeNumberService
    {
        public async Task<CorrelativeNumberEntity> GetByTypeDoc(string typeDoc)
        {
            var sql = """
                SELECT Id, TipoDoc, Numero, RowVersion
                FROM Correlativo WITH (UPDLOCK, HOLDLOCK)
                WHERE TipoDoc = {0}
                """;
            return await context.CorrelativeNumber
                .FromSqlRaw(sql, typeDoc)
                .FirstAsync();
        }
    }
}
