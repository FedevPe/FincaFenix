using FincaFenix.EFCore.Context;
using FincaFenix.Gateways.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FincaFenix.EFCore.Services
{
    public class EfCoreUnitOfWork(FincaFenixContext context) : IUnitOfWork
    {
        private IDbContextTransaction transaction;

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (transaction == null)
            {
                return;
            }

            await transaction.CommitAsync(cancellationToken);
            await transaction.DisposeAsync();
            transaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (transaction == null)
            {
                return;
            }

            await transaction.RollbackAsync(cancellationToken);
            await transaction.DisposeAsync();
            transaction = null;
        }
    }
}
