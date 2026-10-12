using FincaFenix.Entities.DTOs.UserDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.User;

public record CreateUserCommand(CreateUserDTO Dto) : IRequest<UserDTO>;
public record UpdateUserCommand(UpdateUserDTO Dto) : IRequest<UserDTO>;
public record DeactivateUserCommand(string Id) : IRequest<Unit>;
public record ReactivateUserCommand(string Id) : IRequest<UserDTO>;

public class CreateUserHandler(IUserRepository repository) : IRequestHandler<CreateUserCommand, UserDTO>
{
    public async Task<UserDTO> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await repository.Add(request.Dto);
    }
}

public class UpdateUserHandler(IUserRepository repository) : IRequestHandler<UpdateUserCommand, UserDTO>
{
    public async Task<UserDTO> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await repository.Update(request.Dto);
    }
}

public class DeactivateUserHandler(IUserRepository repository) : IRequestHandler<DeactivateUserCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        await repository.Deactivate(request.Id);
        return Unit.Value;
    }
}

public class ReactivateUserHandler(IUserRepository repository) : IRequestHandler<ReactivateUserCommand, UserDTO>
{
    public async Task<UserDTO> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
    {
        return await repository.Reactivate(request.Id);
    }
}