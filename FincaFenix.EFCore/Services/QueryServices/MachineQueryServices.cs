using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class MachineQueryServices(
        FincaFenixContext context) : IMachineQueryService
    {
        public async Task<bool> Exists(int id)
        {
            return await context.Machines.AnyAsync(m => m.Id == id);
        }
        public async Task<IEnumerable<MachineEntity>> GetMachinesAsync()
        {
            return await context.Machines.ToListAsync();
        }
    }
}
