using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IEmployeeQueryService
    {
        Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId);
        Task<(IEnumerable<EmployeeEntity> Employees, int TotalCount)> GetEmployeeListPaged(EmployeeFilterDTO filter);
        Task<EmployeeEntity> GetEmployeeById(int id);
    }
}