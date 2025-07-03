using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IFarmOutputPort
    {
        public FarmDTO Farm { get; }
        public List<FarmDTO> FarmList { get; }
        Task Handle(FarmEntity task);
        Task HandleList(IEnumerable<FarmEntity> materialList);
    }
}
