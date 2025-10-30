using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail
{
    public interface IGetActivitiesWorkOrderOutputPort
    {
        public IEnumerable<DetailWorkOrderDTO> ActivityLog { get; }
        Task HandleList(IEnumerable<DetailWorkOrderEntity> entities);
    }
}
