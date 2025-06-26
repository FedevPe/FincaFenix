using FincaFenix.UsesCases.Interfaces.Sectors;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class SectorInteractor(
        ISectorOutputPort presenter,
        IDetailSectorRepository repository) : ISectorsInputPort
    {
        public async Task GetListSectorsByIdFarm(int farmId)
        {
            await presenter.HandleList(await repository.GetAllSectorsByFarmId(farmId));
        }
        public async Task GetSectorsByOrderId(int orderId)
        {
            await presenter.HandleList(await repository.GetAllSectorsByOrderId(orderId));
        }
    }
}