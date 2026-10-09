using System.Net;
using FincaFenix.Entities;
using FincaFenix.Entities.DTOs.InventoryDTOs.MaterialDTOs;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.UseCases.MaterialCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialCategoryController(ISender mediator) : ControllerBase, IMaterialCategoryController
    {
        [HttpGet("getCategories")]
        [Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_READ)]
        [ProducesResponseType(typeof(IEnumerable<MaterialCategoryDTO>),(int)HttpStatusCode.OK)]
        public async Task<IEnumerable<MaterialCategoryDTO>> GetAllCategories()
        {
            return await mediator.Send(new GetMaterialCategoriesQuery());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_READ)]
        [ProducesResponseType(typeof(MaterialCategoryDTO),(int)HttpStatusCode.OK)]
        public async Task<MaterialCategoryDTO> GetCategoryById(int id)
        {
            return await mediator.Send(new GetMaterialCategoryByIdQuery(id));
        }

        [HttpPost]
        [Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_CREATE)]
        [ProducesResponseType(typeof(MaterialCategoryDTO),(int)HttpStatusCode.Created)]
        public async Task<MaterialCategoryDTO> CreateCategory([FromBody] SaveMaterialCategoryDTO dto)
        {
            return await mediator.Send(new CreateMaterialCategoryCommand(dto));
        }

        [HttpPut]
        [Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_UPDATE)]
        [ProducesResponseType(typeof(MaterialCategoryDTO),(int)HttpStatusCode.OK)]
        public async Task<MaterialCategoryDTO> UpdateCategory([FromBody] SaveMaterialCategoryDTO dto)
        {
            return await mediator.Send(new UpdateMaterialCategoryCommand(dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyMaster.MATERIAL_CATEGORY_DELETE)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<bool> DeleteCategory(int id)
        {
            await mediator.Send(new DeleteMaterialCategoryCommand(id));
            return true;
        }
    }
}
