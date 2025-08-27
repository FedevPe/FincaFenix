using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Mappers;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class DetailWorkOrderInteractor(
        IDetailWorkOrderOutputPort presenter,
        IDetailWorkOrderRepository repository) : IAddDetailWorkOrderInputPort
    {
        public async Task GetActivitiesByOrderId(int orderId)
        {
            await presenter.HandleList(await repository.GetActivityLogByOrderId(orderId));
        }

        public async Task AddDetailWorkOrder(AddDetailWorkOrderDTO dto)
        {
            try
            {
                if (dto != null)
                {
                    var detailWorkOrderEntity = DetailWorkOrderMapper.ToEntity(dto);
                    await presenter.Handle(await repository.CreateDetailWorkOrderAsync(detailWorkOrderEntity));
                }
                else
                {
                    await presenter.Handle(0);
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
