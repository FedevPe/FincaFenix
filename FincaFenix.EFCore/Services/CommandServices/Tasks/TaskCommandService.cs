using FincaFenix.EFCore.Context;
using FincaFenix.Entities.Exceptions;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces;
using FincaFenix.Gateways.Interfaces.CommandServices.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.Tasks
{
    public class TaskCommandService(
        FincaFenixContext context,
        IUnitOfWork unitOfWork) : ITaskCommandService
    {
        public async Task<TaskEntity> CreateAsync(TaskEntity task)
        {
            await unitOfWork.BeginAsync();
            try
            {
                task.Id = 0;
                task.IsDeleted = false;

                await context.Tasks.AddAsync(task);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();

                return task;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<TaskEntity> UpdateAsync(TaskEntity task)
        {
            await unitOfWork.BeginAsync();
            try
            {
                var existing = await context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la tarea con Id {task.Id}.");

                existing.Description = task.Description;
                existing.RendimientoMode = task.RendimientoMode;

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
                var existing = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

                if (existing is null)
                    throw new NotFoundException($"No se encontró la tarea con Id {id}.");

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
    }
}
