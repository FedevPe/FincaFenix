using FincaFenix.Entities.DTOs.RecipeDTO;

namespace FincaFenix.UsesCases.Controllers
{
    public interface IMachineController
    {
        Task<IEnumerable<MachineRecipeDTO>> GetMachines();
    }
}
