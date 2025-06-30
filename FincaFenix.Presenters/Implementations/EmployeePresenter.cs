using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.OutputPort;

namespace FincaFenix.Presenters.Implementations
{
    public class EmployeePresenter : IEmployeeOutputPort
    {
        public IEnumerable<EmployeeDTO>? EmployeeList { get; private set; }

        public Task HandleList(IEnumerable<EmployeeEntity> employeeList)
        {
            EmployeeList = employeeList.Select(e => new EmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                LastName = e.LastName
            }).ToList();

            return Task.CompletedTask;
        }
    }
}
