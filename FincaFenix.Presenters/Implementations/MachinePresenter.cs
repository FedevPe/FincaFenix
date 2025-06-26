using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.UsesCases.Interfaces.Machine;

namespace FincaFenix.Presenters.Implementations
{
    public class MachinePresenter : IMachineOutputPort
    {
        public List<MachineRecipeDTO>? Machines { get; private set; }

        public Task HandleList(IEnumerable<MachineEntity> list)
        {
            Machines = list.Select( machine => new MachineRecipeDTO()
            {
                Id = machine.Id,
                Name = machine.Name,
                Capacity = machine.Capacity,
                CapacityUnit = machine.CapacityUnit

            }).ToList();

            return Task.CompletedTask;
        }
    }
}
