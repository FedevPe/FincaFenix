using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.QueryServices.Login
{
    public interface ILoginQueryService
    {
        Task<UserEntity> ValidateCredentials(LoginDTO login);
    }
}
