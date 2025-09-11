using Microsoft.AspNetCore.Identity;

namespace FincaFenix.Entities.POCOEntities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
