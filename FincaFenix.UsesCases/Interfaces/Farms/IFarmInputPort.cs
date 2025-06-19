using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.Farms
{
    public interface IFarmInputPort
    {
        Task GetListFarm();
        Task GetFarmById(int id);
    }
}
