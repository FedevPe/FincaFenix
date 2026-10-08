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
            return await context.CorrelativeNumber.FirstAsync(c => c.TypeDoc == typeDoc);
        }
    }
}
