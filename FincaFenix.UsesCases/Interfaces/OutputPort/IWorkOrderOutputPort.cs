using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOutputPort
    {
        public InfoWorkOrderDTO WorkOrder { get; }
        public ShowWorkOrderDTO WorkOrderInfo { get; }
        public IEnumerable<WorkOrderDTO> WorkOrderList { get; }
        public List<ShowWorkOrderDTO> InfoWorkOrderList { get; }
        public int TotalCount { get; }
        public bool IsSaved { get; }
        Task Handle(int id);
        Task Handle(InfoWorkOrderDTO workOrderId);
        Task Handle(WorkOrderEntity workOrder);
        Task HandleList(IEnumerable<WorkOrderEntity> listDTO);
        Task HandleListPaginated(IEnumerable<ShowWorkOrderDTO> listDTO, int totalAcount);
    }
}
