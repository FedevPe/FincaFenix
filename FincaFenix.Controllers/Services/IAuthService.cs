using FincaFenix.Entities.DTOs.Login;

namespace FincaFenixControllers.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto);
    }
}
