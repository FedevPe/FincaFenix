using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Farms
{
    public interface IFarmInputPort
    {
        Task Handle(FarmDTO farm);
        Task GetListFarm();
        Task GetFarmById(int id);
    }
}
