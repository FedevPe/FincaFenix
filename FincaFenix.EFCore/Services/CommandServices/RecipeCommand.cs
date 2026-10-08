using FincaFenix.EFCore.Context;
using FincaFenix.Entities.POCOEntities;
using FincaFenix.Gateways.Interfaces.CommandServices;

namespace FincaFenix.EFCore.Services.CommandServices
{
    public class RecipeCommand(FincaFenixContext context) : IRecipeCommand
    {
        public Task AddRecipe(RecipeEntity recipe)
        {
            context.Recipes.Add(recipe);
            return Task.CompletedTask;
        }
    }
}
