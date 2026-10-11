using System.Net;
using FincaFenix.Entities.Config;
using FincaFenix.Entities.DTOs.Login;
using FincaFenixControllers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, IOptions<AuthCookieSettings> cookieSettings) : ControllerBase
    {
        private readonly AuthCookieSettings _cookieSettings = cookieSettings.Value;

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            if (result is null)
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

            Response.Cookies.Append(_cookieSettings.Name, result.Token, BuildCookieOptions(result.ExpiresAt));

            return Ok(result.User);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(_cookieSettings.Name, BuildCookieOptions(null));
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(CurrentUserDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult Me()
        {
            return Ok(authService.GetCurrentUser(User));
        }

        private CookieOptions BuildCookieOptions(DateTime? expiresAt)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = _cookieSettings.Secure,
                SameSite = ParseSameSite(_cookieSettings.SameSite),
                Domain = string.IsNullOrWhiteSpace(_cookieSettings.Domain) ? null : _cookieSettings.Domain,
                Path = "/",
                Expires = expiresAt.HasValue ? new DateTimeOffset(expiresAt.Value, TimeSpan.Zero) : null
            };
        }

        private static SameSiteMode ParseSameSite(string value)
        {
            return Enum.TryParse<SameSiteMode>(value, ignoreCase: true, out var mode) ? mode : SameSiteMode.None;
        }
    }
}
