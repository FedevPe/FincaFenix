using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.UserDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class UserQueryService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        FincaFenixContext context) : IUserQueryService
    {
        public async Task<(IEnumerable<UserDTO> Users, int TotalCount)> GetUserListPagedAsync(UserFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = userManager.Users.AsQueryable();

            if (!filter.IncludeInactive)
                baseQuery = baseQuery.Where(u => u.IsActive);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(u =>
                    u.UserName.Contains(search) ||
                    (u.Email != null && u.Email.Contains(search)) ||
                    (u.Name != null && u.Name.Contains(search)) ||
                    (u.LastName != null && u.LastName.Contains(search)));
            }

            var totalCount = await baseQuery.CountAsync();

            var users = await baseQuery
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            var roleMap = await BuildRoleMapAsync(users.Select(u => u.Id).ToList());

            return (users.Select(u => new UserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Name = u.Name,
                LastName = u.LastName,
                IsActive = u.IsActive,
                Roles = roleMap.TryGetValue(u.Id, out var roles) ? roles : []
            }), totalCount);
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el usuario con Id {id}.");

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

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            return await roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();
        }

        public async Task<bool> UserNameExistsAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName) is not null;
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            return await roleManager.FindByNameAsync(roleName) is not null;
        }

        private async Task<Dictionary<string, List<string>>> BuildRoleMapAsync(List<string> userIds)
        {
            if (userIds.Count == 0)
            {
                return new Dictionary<string, List<string>>();
            }

            var rows = await context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId))
                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, RoleName = r.Name! })
                .ToListAsync();

            return rows
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.RoleName).ToList());
        }
    }
}