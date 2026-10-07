using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Material;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize(Policy = PolicyMaster.MATERIAL_READ)]
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController(ISender mediator) : ControllerBase,IMaterialController
    {
        [HttpGet("category/{categoryId}/material")]
        [ProducesResponseType(typeof(IEnumerable<MaterialRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByCategoryId(int categoryId)
        {
            return await mediator.Send(new GetMaterialListByCategoryQuery(categoryId));
        }
        [HttpGet("recipe/{recipeId}/material")]
        [ProducesResponseType(typeof(IEnumerable<MaterialRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByRecipeId(int recipeId)
        {
            return await mediator.Send(new GetMaterialListByRecipeQuery(recipeId));
        }
        [HttpGet("getmateriallist")]
        [ProducesResponseType(typeof(IEnumerable<MaterialRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetMaterialList()
        {
            return await mediator.Send(new GetMaterialListQuery());
        }
    }
}
