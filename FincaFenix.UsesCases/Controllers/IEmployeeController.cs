using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.EmployeeDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IEmployeeController
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int farmId);
        Task<PagedResult<EmployeeDTO>> GetEmployeeListPaged(EmployeeFilterDTO filter);
        Task<EmployeeDTO> GetEmployeeById(int id);
        Task<EmployeeDTO> CreateEmployee(SaveEmployeeDTO dto);
        Task<EmployeeDTO> UpdateEmployee(SaveEmployeeDTO dto);
        Task<bool> DeleteEmployee(int id);
        Task<EmployeeDTO> ReactivateEmployee(int id);
    }
}