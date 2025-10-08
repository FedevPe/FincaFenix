using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.UsesCases.Repository;
using FluentValidation;

namespace FincaFenix.Validators.Validators.Recipe
{
    public class RecipeValidator : AbstractValidator<RecipeWorkOrderDTO>
    {
        public RecipeValidator(
            IMachineRepository machineRepository,
            IMaterialCategoryRepository categoryRepository,
            IMaterialRepository materialRepository) 
        {
            RuleFor(r => r.MachineId)
                .GreaterThan(0).WithMessage("La receta debe tener una máquina relacionada.");

            RuleFor(r => r.MachineId)
                .MustAsync(async (id, CancellationToken) =>
                {
                    return await machineRepository.Exists(id);
                })
                .WithMessage("La máquina seleccionada no existe en la base de datos.");

            RuleFor(r => r.VolumeMachine)
                .NotEmpty().WithMessage("La receta debe tener un volumen de máquina asociado.")
                .GreaterThanOrEqualTo(0).WithMessage("El volumen de máquina no puede ser menor o igual a 0.");

            RuleFor(r => r.VolumeMachineUnit)
                .NotEmpty().WithMessage("La receta debe tener una unidad de volumen de máquina asociada.");

            RuleFor(r => r.TRV)
                .GreaterThanOrEqualTo(0).WithMessage("El TRV no puede ser menor o igual a 0.");

            RuleForEach(r => r.Details)
                .SetValidator(new DetailRecipeValidator(categoryRepository, materialRepository));
        }
    }
}
