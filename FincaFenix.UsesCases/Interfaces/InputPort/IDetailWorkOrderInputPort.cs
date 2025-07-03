using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;

namespace FincaFenix.UsesCases.Interfaces.InputPort
{
    public interface IDetailWorkOrderInputPort
    {
        Task Handle(AddDetailWorkOrderDTO dto);
    }
}
