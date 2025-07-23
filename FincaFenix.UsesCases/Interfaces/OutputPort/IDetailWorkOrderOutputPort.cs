using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IDetailWorkOrderOutputPort
    {
        public IEnumerable<ActivityWorkOrderDTO> ActivityLog { get; }
        public bool IsSuccess { get; }
        Task Handle(int detailWOId);
        Task HandleList(IEnumerable<DetailWorkOrderEntity> entities);
    }
}
