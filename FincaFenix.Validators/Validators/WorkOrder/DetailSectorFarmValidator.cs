using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Repository;
using FluentValidation;

namespace FincaFenix.Validators.Validators.WorkOrder
{
    public class DetailSectorFarmValidator : AbstractValidator<DetailSectorFarmDTO>
    {
        public DetailSectorFarmValidator(IDetailSectorRepository repository)
        {
            RuleFor(dsf => dsf.Id)
                .GreaterThan(0).WithMessage("El sector seleccionado no es válido.");

            RuleFor(dsf => dsf.Id)
                .MustAsync(async (id, CancellationToken) =>
                {
                    return await repository.Exists(id);
                })
                .WithMessage("El sector seleccionado no existe en la base de datos.");
        }
    }
}
