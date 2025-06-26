using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class InfoCreateOrderViewModel (
        IFarmController farm, 
        ISectorController sector,
        ITaskController task)
    {
        public IEnumerable<FarmDTO> Farms { get; set; } = new List<FarmDTO>();
        public IEnumerable<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();
        public IEnumerable<DetailSectorFarmDTO> Sectors { get; set; } = new List<DetailSectorFarmDTO>();

        public async Task LoadInitializeAsync()
        {
            await Task.WhenAll(LoadFarmsAsync(), LoadTasksAsync());
        }

        public async Task LoadFarmsAsync()
        {
            Farms = await farm.DisplayListFarm();
        }
        public async Task LoadTasksAsync() 
        {
            Tasks = await task.DisplayTaskList();
        }
        public async Task LoadSectorsForFarmIdAsync(int farmId)
        {
            Sectors = await sector.DisplayListSectorByFarmId(farmId);
        }
    }
}
