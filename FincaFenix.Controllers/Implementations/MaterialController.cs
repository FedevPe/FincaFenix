using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController(
        IMaterialInputPort interactor,
        IMaterialOutputPort presenter) : IMaterialController
    {
        [HttpGet("{categoryId}/material")]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByCategoryId(int categoryId)
        {
            await interactor.GetMaterialListByCategoryId(categoryId);
            return presenter.MaterialList;
        }
        [HttpGet("{recipeId}/material")]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByRecipeId(int recipeId)
        {
            await interactor.GetMaterialListByCategoryId(recipeId);
            return presenter.MaterialList;
        }
        [HttpGet("getmateriallist")]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetMaterialList()
        {
            await interactor.GetMaterialList();
            return presenter.MaterialList;
        }
    }
}
