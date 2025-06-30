using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class EmployeeQueryService : FincaFenixContext, IEmployeeQueryService
    {
        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId)
        {
            return await Employee_Farms
                .Where(ef => ef.FarmId == farmId && !ef.Employee.IsDeleted)
                .Select(ef => ef.Employee)
                .ToListAsync();
        }
    }
}
