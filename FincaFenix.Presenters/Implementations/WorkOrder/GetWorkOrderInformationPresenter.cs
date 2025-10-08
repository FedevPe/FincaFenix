using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Aggregates;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;

namespace FincaFenix.Presenters.Implementations.WorkOrder
{
    public class GetWorkOrderInformationPresenter : IGetWorkOrderInformationOutputPort
    {
        public int TotalCount { get; private set; }

        public InfoWorkOrderDTO WorkOrder {get;private set;}

        public ShowWorkOrderDTO WorkOrderInfo {get;private set;}

        public IEnumerable<WorkOrderDTO> WorkOrderList {get;private set;}

        public List<ShowWorkOrderDTO> InfoWorkOrderList {get;private set;}

        public Task Handle(InfoWorkOrderDTO workOrder)
        {
            if (workOrder == null)
                throw new ArgumentNullException(nameof(workOrder), "Work order cannot be null.");
            else
                WorkOrder = workOrder;
            return Task.CompletedTask;
        }
        public async Task Handle(WorkOrderEntity workOrderEntity)
        {
            WorkOrderInfo = WorkOrderMapper.EntityToDTO(workOrderEntity);
            await Task.CompletedTask;
        }
        public Task HandleList(IEnumerable<WorkOrderEntity> listDTO)
        {
            InfoWorkOrderList = listDTO.Select(wo => WorkOrderMapper.EntityToDTO(wo)).ToList();
            return Task.CompletedTask;
        }
        public Task HandleListPaginated(IEnumerable<ShowWorkOrderDTO> listDTO, int totalAcount)
        {
            InfoWorkOrderList = listDTO.ToList();
            TotalCount = totalAcount;
            return Task.CompletedTask;
        }
    }
}
