using FincaFenix.Entities.DTOs.UserDTOs;
using FincaFenix.Gateways.Interfaces.CommandServices.Users;
using FincaFenix.Gateways.Interfaces.QueryServices;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.Gateways.Implementations
{
    public class UserRepository(
        IUserQueryService queryService,
        IUserCommandService commandService) : IUserRepository
    {
        public async Task<(IEnumerable<UserDTO> Users, int TotalCount)> GetUserListPaged(UserFilterDTO filter)
        {
            return await queryService.GetUserListPagedAsync(filter);
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            return await queryService.GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<string>> GetRoles()
        {
            return await queryService.GetRolesAsync();
        }

        public async Task<bool> UserNameExists(string userName)
        {
            return await queryService.UserNameExistsAsync(userName);
        }

        public async Task<bool> RoleNameExists(string roleName)
        {
            return await queryService.RoleNameExistsAsync(roleName);
        }

        public async Task<UserDTO> Add(CreateUserDTO dto)
        {
            return await commandService.CreateAsync(dto);
        }

        public async Task<UserDTO> Update(UpdateUserDTO dto)
        {
            return await commandService.UpdateAsync(dto);
        }

        public async Task Deactivate(string id)
        {
            await commandService.DeactivateAsync(id);
        }

        public async Task<UserDTO> Reactivate(string id)
        {
            return await commandService.ReactivateAsync(id);
        }
    }
}