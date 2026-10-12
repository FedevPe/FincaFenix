using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Employees
{
    public interface IEmployeeCommandService
    {
        Task<EmployeeEntity> CreateAsync(EmployeeEntity employee, List<int> farmIds);
        Task<EmployeeEntity> UpdateAsync(EmployeeEntity employee, List<int> farmIds);
        Task DeleteAsync(int id);
        Task<EmployeeEntity> ReactivateAsync(int id);
    }
}