using AutoMapper;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Employee;

public record GetEmployeeListQuery(int FarmId) : IRequest<IEnumerable<EmployeeDTO>>;

public class GetEmployeeListHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<GetEmployeeListQuery, IEnumerable<EmployeeDTO>>
{
    public async Task<IEnumerable<EmployeeDTO>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
    {
        var employees = await repository.GetAllEmployeesAsync(request.FarmId);
        return mapper.Map<IEnumerable<EmployeeDTO>>(employees);
    }
}
