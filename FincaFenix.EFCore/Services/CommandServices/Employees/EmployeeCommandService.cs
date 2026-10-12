using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Employees;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Employees
{
    public class EmployeeCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : IEmployeeCommandService
    {
        public async Task<EmployeeEntity> CreateAsync(EmployeeEntity employee, List<int> farmIds)
        {
            await unitOfWork.BeginAsync();
            try
            {
                await ValidateFarmsAsync(farmIds);

                employee.Id = 0;
                employee.IsDeleted = false;

                await context.Employees.AddAsync(employee);
                await unitOfWork.SaveChangesAsync();

                foreach (var farmId in farmIds.Distinct())
                {
                    await context.Employee_Farms.AddAsync(new Employee_FarmEntity
                    {
                        EmployeeId = employee.Id,
                        FarmId = farmId
                    });
                }

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return employee;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<EmployeeEntity> UpdateAsync(EmployeeEntity employee, List<int> farmIds)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Employees
                    .Include(e => e.EmployeeList)
                    .FirstOrDefaultAsync(e => e.Id == employee.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró el empleado con Id {employee.Id}.");

                await ValidateFarmsAsync(farmIds);

                existing.Name = employee.Name;
                existing.LastName = employee.LastName;

                context.Employee_Farms.RemoveRange(existing.EmployeeList);

                foreach (var farmId in farmIds)
                {
                    context.Employee_Farms.Add(new Employee_FarmEntity
                    {
                        EmployeeId = existing.Id,
                        FarmId = farmId
                    });
                }

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return existing;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró el empleado con Id {id}.");

                existing.IsDeleted = true;

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<EmployeeEntity> ReactivateAsync(int id)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró el empleado con Id {id}.");

                existing.IsDeleted = false;

                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return existing;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task ValidateFarmsAsync(List<int> farmIds)
        {
            foreach (var farmId in farmIds.Distinct())
            {
                var exists = await context.Farms.AnyAsync(f => f.Id == farmId);

                if (!exists)
                    throw new NotFoundException($"No se encontró la finca con Id {farmId}.");
            }
        }
    }
}