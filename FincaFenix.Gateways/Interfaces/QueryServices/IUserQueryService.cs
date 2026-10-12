using FincaFenix.Entities.DTOs.UserDTOs;

namespace FincaFenix.Gateways.Interfaces.QueryServices
{
    public interface IUserQueryService
    {
        Task<(IEnumerable<UserDTO> Users, int TotalCount)> GetUserListPagedAsync(UserFilterDTO filter);
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<IEnumerable<string>> GetRolesAsync();
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> RoleNameExistsAsync(string roleName);
    }
}