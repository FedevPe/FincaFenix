using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class EmployeeInteractor(
        IEmployeeOutputPort presenter,
        IEmployeeRepository repository) : IEmployeeInputPort
    {
        public async Task GetEmployeeList(int farmId)
        {
            await presenter.HandleList(await repository.GetAllEmployeesAsync(farmId));
        }
    }
}
