using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Mappers;
using FincaFenix.UsesCases.Repository.DetailWorkOrder;

namespace FincaFenix.UsesCases.Interactors.WorkOrderDetail
{
    public class AddDetailWorkOrderInteractor (
        IAddDetailWorkOrderRepository repository,
        IAddDetailWorkOrderOutputPort presenter): IAddDetailWorkOrderInputPort
    {
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
