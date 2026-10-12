using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.UserDTOs;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IUserController
    {
        Task<PagedResult<UserDTO>> GetUserListPaged(UserFilterDTO filter);
        Task<UserDTO> GetUserById(string id);
        Task<IEnumerable<string>> GetRoles();
        Task<UserDTO> CreateUser(CreateUserDTO dto);
        Task<UserDTO> UpdateUser(UpdateUserDTO dto);
        Task<bool> DeactivateUser(string id);
        Task<UserDTO> ReactivateUser(string id);
    }
}