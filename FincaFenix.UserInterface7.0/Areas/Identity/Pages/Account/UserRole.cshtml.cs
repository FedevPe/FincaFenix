using FincaFenix.Entities.POCOEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.UserInterface7._0.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    //[Authorize(Roles = "desarrollador")]
    public class UserRoleModel(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager) : PageModel
    {
        // Propiedad para obtener el rol seleccionado del formulario.
        [BindProperty]
        public IdentityRole Role { get; set; }

        // Propiedad para obtener el ID de usuario seleccionado del formulario.
        [BindProperty]
        public string UserId { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public async Task<IActionResult> OnGet()
        {
            // Obtener todos los roles y usuarios
            var myRoles = await roleManager.Roles.ToListAsync();
            ViewData["roles"] = myRoles;

            // Obtener todos los usuarios y enviarlos a la vista
            Users = await userManager.Users.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Encontrar el usuario por su ID, no por el nombre de la sesión
            var user = await userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                // Manejar el caso si el usuario no es encontrado
                return NotFound($"No se pudo encontrar al usuario con ID '{UserId}'.");
            }

            // Asignar el rol al usuario seleccionado
            var rolassign = await userManager.AddToRoleAsync(user, Role.Name);

            // Redireccionar a una página de éxito
            return RedirectToPage(""); // Puedes cambiar esto a la página que desees.
        }
    }
}

