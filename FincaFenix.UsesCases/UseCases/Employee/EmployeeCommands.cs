using AutoMapper;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Employee;

public record CreateEmployeeCommand(SaveEmployeeDTO Dto) : IRequest<EmployeeDTO>;
public record UpdateEmployeeCommand(SaveEmployeeDTO Dto) : IRequest<EmployeeDTO>;
public record DeleteEmployeeCommand(int Id) : IRequest<Unit>;
public record ReactivateEmployeeCommand(int Id) : IRequest<EmployeeDTO>;

public class CreateEmployeeHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<CreateEmployeeCommand, EmployeeDTO>
{
    public async Task<EmployeeDTO> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<EmployeeEntity>(request.Dto);
        var created = await repository.Add(entity, request.Dto.FarmIds);
        var full = await repository.GetEmployeeById(created.Id);
        return mapper.Map<EmployeeDTO>(full ?? created);
    }
}

public class UpdateEmployeeHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<UpdateEmployeeCommand, EmployeeDTO>
{
    public async Task<EmployeeDTO> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<EmployeeEntity>(request.Dto);
        var updated = await repository.Update(entity, request.Dto.FarmIds);
        var full = await repository.GetEmployeeById(updated.Id);
        return mapper.Map<EmployeeDTO>(full ?? updated);
    }
}

public class DeleteEmployeeHandler(IEmployeeRepository repository) : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        await repository.Delete(request.Id);
        return Unit.Value;
    }
}

public class ReactivateEmployeeHandler(IEmployeeRepository repository, IMapper mapper) : IRequestHandler<ReactivateEmployeeCommand, EmployeeDTO>
{
    public async Task<EmployeeDTO> Handle(ReactivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await repository.Reactivate(request.Id);
        return mapper.Map<EmployeeDTO>(employee);
    }
}