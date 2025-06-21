using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Interfaces.Material;
using FincaFenix.UsesCases.Repository;

namespace FincaFenix.UsesCases.Interactors
{
    public class MaterialInteractor (
        IMaterialOutputPort presenter,
        IMaterialRepository repository): IMaterialInputPort
    {

        public Task GetMaterialById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task GetMaterialList()
        {
            await presenter.HandleList(await repository.GetAllMaterial());
        }
    }
}
