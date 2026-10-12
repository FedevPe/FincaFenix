using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId);
        Task<(IEnumerable<EmployeeEntity> Employees, int TotalCount)> GetEmployeeListPaged(EmployeeFilterDTO filter);
        Task<EmployeeEntity> GetEmployeeById(int id);
        Task<EmployeeEntity> Add(EmployeeEntity employee, List<int> farmIds);
        Task<EmployeeEntity> Update(EmployeeEntity employee, List<int> farmIds);
        Task Delete(int id);
        Task<EmployeeEntity> Reactivate(int id);
    }
}