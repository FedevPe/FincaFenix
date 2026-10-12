using FincaFenix.UsesCases.UseCases.Farm;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Farm
{
    public class CreateFarmCommandValidator : AbstractValidator<CreateFarmCommand>
    {
        public CreateFarmCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("El nombre de la finca es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");
        }
    }
}