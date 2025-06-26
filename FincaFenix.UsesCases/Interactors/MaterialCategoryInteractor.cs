using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class MaterialCategoryInteractor(
        IMaterialCategoryOutputPort presenter,
        IMaterialCategoryRepository repository) : IMaterialCategoryInputPort
    {
        public async Task GetAllCategories()
        {
            await presenter.HandleList(await repository.GetAllCategories());
        }
    }
}
