using System.Net;
using FincaFenix.Entities.DTOs.Login;
using FincaFenixControllers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            if (result is null)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            return Ok(result);
        }
    }
}
