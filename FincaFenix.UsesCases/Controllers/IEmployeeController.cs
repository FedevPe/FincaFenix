using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IEmployeeController
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync(int farmId);
    }
}
