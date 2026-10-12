using FincaFenix.UsesCases.Repository;
using FincaFenix.UsesCases.UseCases.User;
using FluentValidation;

namespace FincaFenix.Validators.Validators.User
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Dto.Id)
                .NotEmpty().WithMessage("El identificador del usuario es obligatorio.");

            RuleFor(x => x.Dto.Name)
                .MaximumLength(50).WithMessage("El nombre no puede superar los 50 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Name));

            RuleFor(x => x.Dto.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede superar los 50 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.LastName));

            RuleFor(x => x.Dto.Email)
                .EmailAddress().WithMessage("El email no tiene un formato válido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dto.Email));

            RuleFor(x => x.Dto.RoleName)
                .NotEmpty().WithMessage("El rol es obligatorio.")
                .MustAsync(async (roleName, cancellationToken) => await userRepository.RoleNameExists(roleName))
                .WithMessage("El rol indicado no existe.");
        }
    }
}