using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IFarmController
    {
        Task<IEnumerable<FarmDTO>> DisplayListFarm();
    }
}
