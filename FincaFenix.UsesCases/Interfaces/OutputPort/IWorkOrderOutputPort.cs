using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOutputPort
    {
        public ShowInfoAddActivityFormDTO WorkOrder { get; }
        public IEnumerable<WorkOrderDTO> WorkOrderList { get; }
        public IEnumerable<ShowWorkOrderCardDTO> InfoWorkOrderCard { get; }
        public int TotalCount { get; }
        public bool IsSaved { get; }
        Task Handle(int workOrderId);
        Task Handle(ShowInfoAddActivityFormDTO workOrder);
        Task HandleList(IEnumerable<ShowWorkOrderCardDTO> listDTO, int totalAcount);
    }
}
