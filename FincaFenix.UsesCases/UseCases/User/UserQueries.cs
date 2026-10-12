using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.UserDTOs;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.User;

public record GetUserListPagedQuery(UserFilterDTO Filter) : IRequest<PagedResult<UserDTO>>;
public record GetUserByIdQuery(string Id) : IRequest<UserDTO>;
public record GetRolesQuery : IRequest<IEnumerable<string>>;

public class GetUserListPagedHandler(IUserRepository repository) : IRequestHandler<GetUserListPagedQuery, PagedResult<UserDTO>>
{
    public async Task<PagedResult<UserDTO>> Handle(GetUserListPagedQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await repository.GetUserListPaged(request.Filter);

        return new PagedResult<UserDTO>
        {
            Items = users,
            TotalCount = totalCount,
            Page = request.Filter.PageNumber,
            PageSize = request.Filter.PageSize
        };
    }
}

public class GetUserByIdHandler(IUserRepository repository) : IRequestHandler<GetUserByIdQuery, UserDTO>
{
    public async Task<UserDTO> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetUserById(request.Id);
    }
}

public class GetRolesHandler(IUserRepository repository) : IRequestHandler<GetRolesQuery, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetRoles();
    }
}