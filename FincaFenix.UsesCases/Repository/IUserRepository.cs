using FincaFenix.Entities.DTOs.UserDTOs;

namespace FincaFenix.UsesCases.Repository
{
    public interface IUserRepository
    {
        Task<(IEnumerable<UserDTO> Users, int TotalCount)> GetUserListPaged(UserFilterDTO filter);
        Task<UserDTO> GetUserById(string id);
        Task<IEnumerable<string>> GetRoles();
        Task<bool> UserNameExists(string userName);
        Task<bool> RoleNameExists(string roleName);
        Task<UserDTO> Add(CreateUserDTO dto);
        Task<UserDTO> Update(UpdateUserDTO dto);
        Task Deactivate(string id);
        Task<UserDTO> Reactivate(string id);
    }
}