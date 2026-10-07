using FincaFenix.Entities.DTOs.Login;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Login
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        public LoginDTOValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("El usuario es obligatorio.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");
        }
    }
}
