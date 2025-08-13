using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class SearchBarViewModel (
        ITaskController task,
        IFarmController farm)
    {
        public string? SearchTerm { get; set; }
        public int FincaId { get; set; }
        public int TaskId { get; set; }
        public string? State { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IEnumerable<FarmDTO> Farms { get; set; } = new List<FarmDTO>();
        public IEnumerable<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();

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
    }
}
