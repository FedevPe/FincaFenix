using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository.Login
{
    public interface ILoginRepository
    {
        Task<UserEntity> ValidateCredentials(LoginDTO login);
    }
}
