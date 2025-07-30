using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class NewOrderDataViewModel (
        IFarmController farm,
        IDetailSectorController sector,
        ITaskController task)
    {
        public int SelectedFarmId { get; set; }
        public int SelectedTaskId { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public List<DetailSectorFarmDTO> SelectedSectors { get; set; } = new List<DetailSectorFarmDTO>();

        public decimal? TotalAreaSectors { get; set; } = 0;

        public IEnumerable<FarmDTO> Farms { get; set; } = new List<FarmDTO>();
        public IEnumerable<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();
        public IEnumerable<DetailSectorFarmDTO> Sectors { get; set; } = new List<DetailSectorFarmDTO>();

        public async Task LoadInitializeAsync()
        {
            await Task.WhenAll(LoadFarmsAsync(), LoadTasksAsync());
        }
        public async Task LoadFarmsAsync()
        {
            Farms = await farm.GetListFarm();
        }
        public async Task LoadTasksAsync() 
        {
            Tasks = await task.DisplayTaskList();
        }
        public async Task LoadSectorsForFarmIdAsync(int farmId)
        {
            Sectors = await sector.GetListSectorByFarmId(farmId);
        }
    }
}
