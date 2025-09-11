using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations.Login
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController(
        SignInManager<ApplicationUser> signInManager) : ControllerBase
    {
        [HttpPost("validatecredentials")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await signInManager.PasswordSignInAsync(
                         dto.UserName, dto.Password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Devuelve un objeto con IsSuccess = true.
                return Ok(new { IsSuccess = true });
            }

            // Devuelve un objeto con IsSuccess = false.
            return Unauthorized(new { IsSuccess = false });
        }
    }
}
