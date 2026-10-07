using AutoMapper;
using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.UsesCases.Repository;
using MediatR;

namespace FincaFenix.UsesCases.UseCases.Machine;

public record GetMachineListQuery : IRequest<IEnumerable<MachineRecipeDTO>>;

public class GetMachineListHandler(IMachineRepository repository, IMapper mapper) : IRequestHandler<GetMachineListQuery, IEnumerable<MachineRecipeDTO>>
{
    public async Task<IEnumerable<MachineRecipeDTO>> Handle(GetMachineListQuery request, CancellationToken cancellationToken)
    {
        var machines = await repository.GetMachineList();
        return mapper.Map<IEnumerable<MachineRecipeDTO>>(machines);
    }
}
