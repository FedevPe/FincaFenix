using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface IFarmInputPort
    {
        Task GetListFarm();
        Task GetFarmById(int id);
    }
}
