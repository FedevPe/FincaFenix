using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetailWorkOrderController (
        IDetailWorkOrderInputPort interactor,
        IDetailWorkOrderOutputPort presenter) : IDetailWorkOrderController
    {
        [HttpPost("addDetailWO")]
        public async Task<bool> CreateDetailWorkOrder(AddDetailWorkOrderDTO dto)
        {
            await interactor.Handle(dto);
            return presenter.IsSuccess;
        }
    }
}
