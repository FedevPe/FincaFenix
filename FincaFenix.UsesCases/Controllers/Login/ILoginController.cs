using FincaFenix.Entities.DTOs.Login;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenix.UsesCases.Controllers.Login
{
    public interface ILoginController
    {
        Task<IActionResult> ValidateCredentials(LoginDTO credentials);
    }
}
