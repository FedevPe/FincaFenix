using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Interfaces.Machine
{
    public interface IMachineOutputPort
    {
        public List<MachineRecipeDTO> Machines { get; }
        Task HandleList(IEnumerable<MachineEntity> list);
    }
}
