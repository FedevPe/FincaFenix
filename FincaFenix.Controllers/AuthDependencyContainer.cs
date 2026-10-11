using System.Reflection;
using System.Text;
using FincaFenix.Entities;
using FincaFenix.Entities.Config;
using FincaFenixControllers.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthDependencyContainer
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);

            var cookieSection = configuration.GetSection("AuthCookie");
            services.Configure<AuthCookieSettings>(cookieSection);

            var jwtSettings = jwtSection.Get<JwtSettings>();
            var cookieSettings = cookieSection.Get<AuthCookieSettings>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue(cookieSettings?.Name ?? "FincaFenix.Auth", out var token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            var allPolicies = typeof(PolicyMaster)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null))
                .ToList();

            services.AddAuthorization(options =>
            {
                foreach (var policy in allPolicies)
                {
                    options.AddPolicy(policy, builder =>
                        builder.RequireClaim(CustomClaims.POLICIES, policy));
                }
            });

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
