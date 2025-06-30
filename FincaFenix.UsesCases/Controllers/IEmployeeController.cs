using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IEmployeeController
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int farmId);
    }
}
