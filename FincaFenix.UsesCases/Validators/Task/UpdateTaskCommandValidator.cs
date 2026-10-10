using FincaFenix.Entities.Enum;
using FincaFenix.UsesCases.UseCases.Task;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Task
{
    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.Dto.Id)
                .NotNull().WithMessage("La tarea es obligatoria.")
                .GreaterThan(0).WithMessage("La tarea es obligatoria.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("La descripción de la tarea es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede superar los 100 caracteres.");

            RuleFor(x => x.Dto.RendimientoMode)
                .NotEmpty().WithMessage("El modo de rendimiento es obligatorio.")
                .Must(BeValidMode).WithMessage("El modo de rendimiento no es válido.");
        }

        private static bool BeValidMode(string value)
        {
            return Enum.TryParse<RendimientoModeEnum>(value, true, out _);
        }
    }
}
