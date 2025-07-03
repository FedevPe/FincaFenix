using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IDetailWorkOrderOutputPort
    {
        public bool IsSuccess { get; }
        Task Handle(int detailWOId);
    }
}
