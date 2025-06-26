using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.InputPort;
using FincaFenix.UsesCases.Interfaces.OutputPort;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialCategoryController(
        IMaterialCategoryInputPort interactor,
        IMaterialCategoryOutputPort presenter) : IMaterialCategoryController
    {
        [HttpGet("getCategories")]
        public async Task<IEnumerable<MaterialCategoryDTO>> GetAllCategories()
        {
            await interactor.GetAllCategories();
            return presenter.Categories;
        }
    }
}
