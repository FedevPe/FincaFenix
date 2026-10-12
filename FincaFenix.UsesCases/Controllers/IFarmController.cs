using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.FarmDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IFarmController
    {
        Task<IEnumerable<FarmDTO>> GetListFarm();
        Task<PagedResult<FarmDTO>> GetFarmListPaged(FarmFilterDTO filter);
        Task<FarmDTO> GetFarmById(int id);
        Task<FarmDTO> CreateFarm(SaveFarmDTO dto);
        Task<FarmDTO> UpdateFarm(SaveFarmDTO dto);
        Task<bool> DeleteFarm(int id);
        Task<FarmDTO> ReactivateFarm(int id);
    }
}