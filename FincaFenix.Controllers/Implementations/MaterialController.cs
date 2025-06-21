using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;
using FincaFenix.UsesCases.Interfaces.Material;
using Microsoft.AspNetCore.Mvc;

namespace FincaFenixControllers.Implementations
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController(
        IMaterialInputPort interactor,
        IMaterialOutputPort presenter) : IMaterialController
    {
        [HttpGet("GetMateriallist")]
        public async Task<IEnumerable<MaterialDTO>> DisplayListMaterial()
        {
            await interactor.GetMaterialList();
            return presenter.MaterialList;
        }

        public Task<MaterialDTO> GetMaterialById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
