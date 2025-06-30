using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IEmployeeQueryService
    {
        Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId);
    }
}
