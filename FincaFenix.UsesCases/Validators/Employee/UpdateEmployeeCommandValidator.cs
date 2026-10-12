using FincaFenix.UsesCases.UseCases.Employee;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Employee
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("El empleado es obligatorio.")
                .GreaterThan(0).WithMessage("El empleado es obligatorio.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("El nombre del empleado es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar los 50 caracteres.");

            RuleFor(x => x.Dto.LastName)
                .NotEmpty().WithMessage("El apellido del empleado es obligatorio.")
                .MaximumLength(50).WithMessage("El apellido no puede superar los 50 caracteres.");

            RuleFor(x => x.Dto.FarmIds)
                .NotNull().WithMessage("Debe asignar al menos una finca.")
                .Must(farms => farms != null && farms.Count > 0).WithMessage("Debe asignar al menos una finca.");
        }
    }
}