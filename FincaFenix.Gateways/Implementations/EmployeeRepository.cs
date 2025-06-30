using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class EmployeeRepository (IEmployeeQueryService service) : IEmployeeRepository
    {
        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId)
        {
            return await service.GetAllEmployeesAsync(farmId);
        }
    }
}
