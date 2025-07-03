using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IWorkOrderRepository
    {
        Task<int> AddWorkOrder(WorkOrderEntity workOrder, RecipeEntity recipe, List<WorkOrderWorkedSectorEntity> workedSectors);
        Task<ShowInfoAddActivityFormDTO> GetWorkOrderById(int id);
        Task<(IEnumerable<ShowWorkOrderCardDTO> WorkOrders, int TotalAcount)> GetWorkOrderList(int pageNumber, int pageSize);
    }
}
