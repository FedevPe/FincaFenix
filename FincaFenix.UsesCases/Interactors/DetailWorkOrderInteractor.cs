using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Mappers;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class DetailWorkOrderInteractor(
        IDetailWorkOrderOutputPort presenter,
        IDetailWorkOrderRepository repository) : IDetailWorkOrderInputPort
    {
        public async Task GetActivitiesByOrderId(int orderId)
        {
            await presenter.HandleList(await repository.GetActivityLogByOrderId(orderId));
        }

        public async Task Handle(AddDetailWorkOrderDTO dto)
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
