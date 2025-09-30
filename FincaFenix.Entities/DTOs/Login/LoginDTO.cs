using System.ComponentModel.DataAnnotations;

namespace FincaFenix.Entities.DTOs.Login
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
