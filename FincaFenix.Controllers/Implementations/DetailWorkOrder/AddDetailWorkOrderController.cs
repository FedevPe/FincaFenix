using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrderDetail;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrderDetail;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.DetailWorkOrder
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddDetailWorkOrderController(
        IAddDetailWorkOrderInputPort interactor,
        IAddDetailWorkOrderOutputPort presenter) : IAddDetailWorkOrderController
    {
        [HttpPost("addDetailWO")]
        public async Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto)
        {
            await interactor.AddDetailWorkOrder(dto);
            return presenter.IsSuccess;
        }
    }
}
