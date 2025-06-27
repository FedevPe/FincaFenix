using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IWorkOrderRepository
    {
        Task<int> AddWorkOrder(WorkOrderEntity workOrder, RecipeEntity recipe, List<WorkOrderWorkedSectorEntity> workedSectors);
        Task<string> GetLastNumberDoc(string typeDoc);
        Task<WorkOrderEntity> GetWorkOrderByOrderNum(int orderNum);
        Task<IEnumerable<WorkOrderEntity>> GetListWorkOrderByFarmId(int farmId);
        Task<IEnumerable<WorkOrderEntity>> GetListWorkOrderByTaskId(int taskId);
    }
}
