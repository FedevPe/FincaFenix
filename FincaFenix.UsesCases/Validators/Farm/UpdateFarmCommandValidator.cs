using FincaFenix.UsesCases.UseCases.Farm;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Farm
{
    public class UpdateFarmCommandValidator : AbstractValidator<UpdateFarmCommand>
    {
        public UpdateFarmCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("La finca es obligatoria.")
                .GreaterThan(0).WithMessage("La finca es obligatoria.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("El nombre de la finca es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");
        }
    }
}