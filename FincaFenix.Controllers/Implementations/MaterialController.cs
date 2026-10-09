using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.Material;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController(ISender mediator) : ControllerBase,IMaterialController
    {
        [HttpGet("category/{categoryId}/material")]
        [Authorize(Policy = PolicyMaster.MATERIAL_READ)]
        [ProducesResponseType(typeof(IEnumerable<MaterialRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByCategoryId(int categoryId)
        {
            return await mediator.Send(new GetMaterialListByCategoryQuery(categoryId));
        }

        [HttpGet("recipe/{recipeId}/material")]
        [Authorize(Policy = PolicyMaster.MATERIAL_READ)]
        [ProducesResponseType(typeof(IEnumerable<MaterialRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetListMaterialByRecipeId(int recipeId)
        {
            return await mediator.Send(new GetMaterialListByRecipeQuery(recipeId));
        }

        [HttpGet("getmateriallist")]
        [Authorize(Policy = PolicyMaster.MATERIAL_READ)]
        [ProducesResponseType(typeof(IEnumerable<MaterialRecipeDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialRecipeDTO>> GetMaterialList()
        {
            return await mediator.Send(new GetMaterialListQuery());
        }

        [HttpGet("paged")]
        [Authorize(Policy = PolicyMaster.MATERIAL_READ)]
        [ProducesResponseType(typeof(PagedResult<MaterialDTO>),(int)HttpStatusCode.OK)]
        public async Task<PagedResult<MaterialDTO>> GetMaterialListPaged([FromQuery] MaterialFilterDTO filter)
        {
            return await mediator.Send(new GetMaterialListPagedQuery(filter));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.MATERIAL_READ)]
        [ProducesResponseType(typeof(MaterialDTO),(int)HttpStatusCode.OK)]
        public async Task<MaterialDTO> GetMaterialById(int id)
        {
            return await mediator.Send(new GetMaterialByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.MATERIAL_CREATE)]
        [ProducesResponseType(typeof(MaterialDTO),(int)HttpStatusCode.Created)]
        public async Task<MaterialDTO> CreateMaterial([FromBody] CreateMaterialDTO dto)
        {
            return await mediator.Send(new CreateMaterialCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.MATERIAL_UPDATE)]
        [ProducesResponseType(typeof(MaterialDTO),(int)HttpStatusCode.OK)]
        public async Task<MaterialDTO> UpdateMaterial([FromBody] UpdateMaterialDTO dto)
        {
            return await mediator.Send(new UpdateMaterialCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.MATERIAL_DELETE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeleteMaterial(int id)
        {
            await mediator.Send(new DeleteMaterialCommand(id));
            return true;
        }
    }
}
