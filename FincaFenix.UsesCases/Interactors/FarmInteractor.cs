using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Farms;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class FarmInteractor(IFarmOutputPort presenter,
        IFarmRepository repository) : IFarmInputPort
    {
        public async Task GetFarmById(int id)
        {
            await presenter.Handle(await repository.GetFarmById(id));
        }
        public async Task GetListFarm()
        {
            await presenter.HandleList(await repository.GetListFarm());
        }
    }
}
