namespace UniversitarySystem.Entities.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChanges();
    }
}
