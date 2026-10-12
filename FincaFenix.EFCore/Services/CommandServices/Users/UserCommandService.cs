using FincaFenix.Entities.DTOs.UserDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Users;
using Microsoft.AspNetCore.Identity;

namespace FincaFenix.EFCore.Services.CommandServices.Users
{
    public class UserCommandService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUnitOfWork unitOfWork) : IUserCommandService
    {
        public async Task<UserDTO> CreateAsync(CreateUserDTO dto)
        {
            await unitOfWork.BeginAsync();
            try
            {
                if (await userManager.FindByNameAsync(dto.UserName) is not null)
                    throw new BusinessRuleException($"Ya existe un usuario con el nombre '{dto.UserName}'.");

                if (await roleManager.FindByNameAsync(dto.RoleName) is null)
                    throw new BusinessRuleException($"El rol '{dto.RoleName}' no existe.");

                var user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    Name = dto.Name,
                    LastName = dto.LastName,
                    IsActive = true
                };

                var createResult = await userManager.CreateAsync(user, dto.Password);
                if (!createResult.Succeeded)
                    throw new BusinessRuleException(FormatErrors(createResult));

                var roleResult = await userManager.AddToRoleAsync(user, dto.RoleName);
                if (!roleResult.Succeeded)
                    throw new BusinessRuleException(FormatErrors(roleResult));

                await unitOfWork.CommitAsync();

                var roles = await userManager.GetRolesAsync(user);

                return new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    Roles = roles.ToList()
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<UserDTO> UpdateAsync(UpdateUserDTO dto)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var user = await userManager.FindByIdAsync(dto.Id)
                    ?? throw new NotFoundException($"No se encontró el usuario con Id {dto.Id}.");

                if (await roleManager.FindByNameAsync(dto.RoleName) is null)
                    throw new BusinessRuleException($"El rol '{dto.RoleName}' no existe.");

                user.Email = dto.Email;
                user.Name = dto.Name;
                user.LastName = dto.LastName;

                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new BusinessRuleException(FormatErrors(updateResult));

                var currentRoles = await userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(dto.RoleName))
                {
                    if (currentRoles.Count > 0)
                    {
                        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                        if (!removeResult.Succeeded)
                            throw new BusinessRuleException(FormatErrors(removeResult));
                    }

                    var addResult = await userManager.AddToRoleAsync(user, dto.RoleName);
                    if (!addResult.Succeeded)
                        throw new BusinessRuleException(FormatErrors(addResult));
                }

                await unitOfWork.CommitAsync();

                var roles = await userManager.GetRolesAsync(user);

                return new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    Roles = roles.ToList()
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeactivateAsync(string id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id)
                    ?? throw new NotFoundException($"No se encontró el usuario con Id {id}.");

                user.IsActive = false;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new BusinessRuleException(FormatErrors(result));

                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<UserDTO> ReactivateAsync(string id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id)
                    ?? throw new NotFoundException($"No se encontró el usuario con Id {id}.");

                user.IsActive = true;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new BusinessRuleException(FormatErrors(result));

                await unitOfWork.CommitAsync();

                var roles = await userManager.GetRolesAsync(user);

                return new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    Roles = roles.ToList()
                };
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private static string FormatErrors(IdentityResult result)
        {
            return string.Join(" ", result.Errors.Select(e => e.Description));
        }
    }
}