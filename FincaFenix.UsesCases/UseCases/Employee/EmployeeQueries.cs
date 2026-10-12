using AutoMapper;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Employee;

public record GetEmployeeListQuery(int FarmId) : IRequest<IEnumerable<EmployeeDTO>>;
public record GetEmployeeListPagedQuery(EmployeeFilterDTO Filter) : IRequest<PagedResult<EmployeeDTO>>;
public record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDTO>;

public class GetEmployeeListHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<GetEmployeeListQuery, IEnumerable<EmployeeDTO>>
{
    public async Task<IEnumerable<EmployeeDTO>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
    {
        var employees = await repository.GetAllEmployeesAsync(request.FarmId);
        return mapper.Map<IEnumerable<EmployeeDTO>>(employees);
    }
}

public class GetEmployeeListPagedHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<GetEmployeeListPagedQuery, PagedResult<EmployeeDTO>>
{
    public async Task<PagedResult<EmployeeDTO>> Handle(GetEmployeeListPagedQuery request, CancellationToken cancellationToken)
    {
        var (employees, totalCount) = await repository.GetEmployeeListPaged(request.Filter);

        return new PagedResult<EmployeeDTO>
        {
            Items = mapper.Map<IEnumerable<EmployeeDTO>>(employees),
            TotalCount = totalCount,
            Page = request.Filter.PageNumber,
            PageSize = request.Filter.PageSize
        };
    }
}

public class GetEmployeeByIdHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<GetEmployeeByIdQuery, EmployeeDTO>
{
    public async Task<EmployeeDTO> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await repository.GetEmployeeById(request.Id);
        return mapper.Map<EmployeeDTO>(employee);
    }
}