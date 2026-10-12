using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices.Employees;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class EmployeeRepository(
        IEmployeeQueryService queryService,
        IEmployeeCommandService commandService) : IEmployeeRepository
    {
        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId)
        {
            return await queryService.GetAllEmployeesAsync(farmId);
        }

        public async Task<(IEnumerable<EmployeeEntity> Employees, int TotalCount)> GetEmployeeListPaged(EmployeeFilterDTO filter)
        {
            return await queryService.GetEmployeeListPaged(filter);
        }

        public async Task<EmployeeEntity> GetEmployeeById(int id)
        {
            return await queryService.GetEmployeeById(id);
        }

        public async Task<EmployeeEntity> Add(EmployeeEntity employee, List<int> farmIds)
        {
            return await commandService.CreateAsync(employee, farmIds);
        }

        public async Task<EmployeeEntity> Update(EmployeeEntity employee, List<int> farmIds)
        {
            return await commandService.UpdateAsync(employee, farmIds);
        }

        public async Task Delete(int id)
        {
            await commandService.DeleteAsync(id);
        }

        public async Task<EmployeeEntity> Reactivate(int id)
        {
            return await commandService.ReactivateAsync(id);
        }
    }
}