using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class MachineRepository(IMachineQueryService service) : IMachineRepository
    {
        public async Task<bool> Exists(int id)
        {
            return await service.Exists(id);
        }

        public async Task<IEnumerable<MachineEntity>> GetMachineList()
        {
            return await service.GetMachinesAsync();
        }
    }
}
