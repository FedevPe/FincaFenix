using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder
{
    public interface IGetWorkOrderInformationOutputPort
    {
        public int TotalCount { get; }
        public InfoWorkOrderDTO WorkOrder { get; }
        public ShowWorkOrderDTO WorkOrderInfo { get; }
        public IEnumerable<WorkOrderDTO> WorkOrderList { get; }
        public List<ShowWorkOrderDTO> InfoWorkOrderList { get; }
        Task Handle(InfoWorkOrderDTO workOrderId);
        Task Handle(WorkOrderEntity workOrder);
        Task HandleList(IEnumerable<WorkOrderEntity> listDTO);
        Task HandleListPaginated(IEnumerable<ShowWorkOrderDTO> listDTO, int totalAcount);
    }
}
