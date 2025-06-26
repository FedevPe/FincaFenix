using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class MaterialInteractor(
        IMaterialOutputPort presenter,
        IMaterialRepository repository) : IMaterialInputPort
    {
        public async Task GetMaterialList()
        {
            await presenter.HandleList(await repository.GetMaterialList());
        }

        public async Task GetMaterialListByCategoryId(int categoryId)
        {
            await presenter.HandleList(await repository.GetAllMaterialByCategoryId(categoryId));
        }

        public Task GetMaterialListByRecipeId(int recipeId)
        {
            throw new NotImplementedException();
        }
    }
}
