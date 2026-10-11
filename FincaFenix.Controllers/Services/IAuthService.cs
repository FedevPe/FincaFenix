using System.Security.Claims;
using FincaFenix.Entities.DTOs.Login;

namespace FincaFenixControllers.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto);
        CurrentUserDTO GetCurrentUser(ClaimsPrincipal principal);
    }
}
