using FincaFenix.UsesCases.Repository;
using FincaFenix.UsesCases.UseCases.User;
using FluentValidation;

namespace FincaFenix.Validators.Validators.User
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Dto.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MaximumLength(256).WithMessage("El nombre de usuario no puede superar los 256 caracteres.")
                .MustAsync(async (userName, cancellationToken) => !await userRepository.UserNameExists(userName))
                .WithMessage("Ya existe un usuario con ese nombre.");

            RuleFor(x => x.Dto.Email)
                .EmailAddress().WithMessage("El email no tiene un formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un dígito.")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula.")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula.");

            RuleFor(x => x.Dto.RoleName)
                .NotEmpty().WithMessage("El rol es obligatorio.")
                .MustAsync(async (roleName, cancellationToken) => await userRepository.RoleNameExists(roleName))
                .WithMessage("El rol indicado no existe.");
        }
    }
}