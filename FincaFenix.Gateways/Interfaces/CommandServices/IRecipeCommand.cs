using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.Gateways.Interfaces.CommandServices
{
    public interface IRecipeCommand
    {
        Task AddRecipe(RecipeEntity recipe);
    }
}
