using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.Farms
{
    public interface IFarmOutputPort
    {
        Task Handle(FarmEntity task);
        Task HandleList(IEnumerable<FarmEntity> materialList);
    }
}
