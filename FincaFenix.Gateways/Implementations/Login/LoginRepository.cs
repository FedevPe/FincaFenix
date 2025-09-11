using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.QueryServices.Login;
using FincaFenix.UsesCases.Repository.Login;

namespace FincaFenix.Gateways.Implementations.Login
{
    public class LoginRepository(
        ILoginQueryService query) : ILoginRepository
    {
        public async Task<UserEntity> ValidateCredentials(LoginDTO login)
        {
            return await query.ValidateCredentials(login);
        }
    }
}
