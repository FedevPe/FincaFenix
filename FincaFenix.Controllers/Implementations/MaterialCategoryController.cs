using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.MaterialCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialCategoryController(ISender mediator) : ControllerBase, IMaterialCategoryController
    {
        [HttpGet("getCategories")]
        [ProducesResponseType(typeof(IEnumerable<MaterialCategoryDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialCategoryDTO>> GetAllCategories()
        {
            return await mediator.Send(new GetMaterialCategoriesQuery());
        }
    }
}
