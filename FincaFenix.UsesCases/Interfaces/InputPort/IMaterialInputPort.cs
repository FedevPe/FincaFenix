namespace FincaFenix.UsesCases.Interfaces.Material
{
    public interface IMaterialInputPort
    {
        Task GetMaterialListByCategoryId(int categoryId);
        Task GetMaterialListByRecipeId(int recipeId);
        Task GetMaterialList();
    }
}
