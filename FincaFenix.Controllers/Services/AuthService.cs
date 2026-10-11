using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FincaFenix.Entities.Config;
using FincaFenix.Entities.DTOs.Login;
using FincaFenix.Entities.POCOEntities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using FincaFenix.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FincaFenixControllers.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings,
            IValidator<LoginDTO> loginValidator,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _loginValidator = loginValidator;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogWarning("Login failed for user {UserName}", loginDto.UserName);
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
                claims.Add(new(ClaimTypes.Role, role));

            var policies = new List<string>();
            var policyClaimValues = new HashSet<string>();
            foreach (var roleName in roles)
            {
                var identityRole = await _roleManager.FindByNameAsync(roleName);
                if (identityRole is null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                foreach (var claim in roleClaims.Where(c => c.Type == CustomClaims.POLICIES))
                {
                    if (policyClaimValues.Add(claim.Value))
                    {
                        claims.Add(claim);
                        policies.Add(claim.Value);
                    }
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            _logger.LogInformation("User {UserName} logged in successfully", loginDto.UserName);

            return new LoginResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                User = new CurrentUserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Policies = policies,
                }
            };
        }

        public CurrentUserDTO GetCurrentUser(ClaimsPrincipal principal)
        {
            if (principal?.Identity?.IsAuthenticated != true)
                return null;

            return new CurrentUserDTO
            {
                Id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                UserName = principal.FindFirst(ClaimTypes.Name)?.Value,
                Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToList(),
                Policies = principal.FindAll(CustomClaims.POLICIES).Select(c => c.Value).Distinct().ToList()
            };
        }
    }
}
