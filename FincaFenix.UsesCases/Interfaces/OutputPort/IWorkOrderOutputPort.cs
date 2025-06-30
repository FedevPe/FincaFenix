using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.WorkOrder
{
    public interface IWorkOrderOutputPort
    {
        IEnumerable<WorkOrderDTO> WorkOrderList { get; }
        public bool IsSaved { get; }
        Task Handle(int workOrderId);
        Task HandleList(IEnumerable<WorkOrderEntity> workOrdersEntity, IEnumerable<DetailSectorFarmEntity> sectorList);
    }
}
