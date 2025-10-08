using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.InversionOfControl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;

namespace FincaFenix.UserInterface7._0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var defaultCulture = "es-AR";
            var culture = new CultureInfo(defaultCulture);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
            {
                // Solo habilita errores detallados en el entorno de desarrollo
                options.DetailedErrors = builder.Environment.IsDevelopment();
            });
            builder.Services.AddControllers();
            builder.Services.AddServicesContainer();
            builder.Services.AddUIServices();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomStart;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                config.SnackbarConfiguration.VisibleStateDuration = 3000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
            });

            var conn = builder.Configuration.GetConnectionString("DefaultConnection");
            // Agregar el DbContext de Entity Framework Core para Identity
            builder.Services.AddDbContext<FincaFenixContext>(conf => conf.UseSqlServer(conn));

            //Agregar Identity y su configuraci�n.
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;

                    options.SignIn.RequireConfirmedEmail = false;

                    options.Lockout.AllowedForNewUsers = false;
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<FincaFenixContext>()
                .AddRoles<IdentityRole>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/accesodenegado";

                // Expira la sesion tras 1 hora de inactividad
                options.ExpireTimeSpan = TimeSpan.FromHours(1);

                // SlidingExpiration = false ⇒ no se renueva automáticamente
                options.SlidingExpiration = true;

                // (opcional) Evita que la cookie se mantenga tras cerrar el navegador
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages().RequireAuthorization();

            app.MapControllers();

            app.MapBlazorHub().RequireAuthorization();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}
