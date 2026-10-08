using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder
{
    public interface ICorrelativeNumberService
    {
        Task<CorrelativeNumberEntity> GetByTypeDoc(string typeDoc);
    }
}
