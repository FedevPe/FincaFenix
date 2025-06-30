using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.OutputPort
{
    public interface IEmployeeOutputPort
    {
        public IEnumerable<EmployeeDTO> EmployeeList { get; }
        Task HandleList(IEnumerable<EmployeeEntity> employeeList);
    }
}
