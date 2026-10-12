using FincaFenix.Entities.DTOs.UserDTOs;

namespace FincaFenix.Gateways.Interfaces.CommandServices.Users
{
    public interface IUserCommandService
    {
        Task<UserDTO> CreateAsync(CreateUserDTO dto);
        Task<UserDTO> UpdateAsync(UpdateUserDTO dto);
        Task DeactivateAsync(string id);
        Task<UserDTO> ReactivateAsync(string id);
    }
}