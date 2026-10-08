namespace FincaFenix.Gateways.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
