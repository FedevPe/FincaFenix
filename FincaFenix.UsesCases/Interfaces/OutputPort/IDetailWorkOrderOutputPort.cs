using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IDetailWorkOrderOutputPort
    {
        public IEnumerable<DetailWorkOrderDTO> ActivityLog { get; }
        public bool IsSuccess { get; }
        Task Handle(int detailWOId);
        Task HandleList(IEnumerable<DetailWorkOrderEntity> entities);
    }
}
