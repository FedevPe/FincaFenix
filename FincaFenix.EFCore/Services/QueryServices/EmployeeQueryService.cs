using FincaFenix.EFCore.Context;
using FincaFenix.Entities.DTOs.EmployeeDTOs;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.QueryServices
{
    public class EmployeeQueryService(
        FincaFenixContext context) : IEmployeeQueryService
    {
        public async Task<IEnumerable<EmployeeEntity>> GetAllEmployeesAsync(int farmId)
        {
            return await context.Employee_Farms
                .Where(ef => ef.FarmId == farmId && !ef.Employee.IsDeleted)
                .Select(ef => ef.Employee)
                .ToListAsync();
        }

        public async Task<EmployeeEntity> GetEmployeeById(int id)
        {
            return await context.Employees
                .Include(e => e.EmployeeList)
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException($"No se encontró el empleado con Id {id}.");
        }

        public async Task<(IEnumerable<EmployeeEntity> Employees, int TotalCount)> GetEmployeeListPaged(EmployeeFilterDTO filter)
        {
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var baseQuery = context.Employees.AsQueryable();

            if (!filter.IncludeDeleted)
                baseQuery = baseQuery.Where(e => !e.IsDeleted);

            if (filter.FarmId.HasValue)
                baseQuery = baseQuery.Where(e => e.EmployeeList.Any(ef => ef.FarmId == filter.FarmId.Value));

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                baseQuery = baseQuery.Where(e =>
                    e.Name.Contains(search) ||
                    (e.LastName != null && e.LastName.Contains(search)));
            }

            var totalCount = await baseQuery.CountAsync();

            var employees = await baseQuery
                .Include(e => e.EmployeeList)
                .OrderBy(e => e.Name)
                .ThenBy(e => e.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (employees, totalCount);
        }
    }
}