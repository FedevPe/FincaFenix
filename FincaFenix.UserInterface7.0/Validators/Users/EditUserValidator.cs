using FincaFenix.ViewModels.ViewModels.User;
using FluentValidation;

namespace FincaFenix.UserInterface7._0.Validators.Users
{
    public class EditUserValidator : AbstractValidator<EditUserViewModel>
    {
        public EditUserValidator()
        {
            When(x => !string.IsNullOrWhiteSpace(x.NewPassword), () =>
            {
                RuleFor(x => x.NewPassword)
                    .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                    .Matches("[0-9]").WithMessage("Debe contener al menos un número.")
                    .Matches("[a-z]").WithMessage("Debe contener al menos una letra minúscula.")
                    .Matches("[A-Z]").WithMessage("Debe contener al menos una letra mayúscula.");

                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword)
                    .WithMessage("Las contraseñas no coinciden.");
            });
        }
    }
}
