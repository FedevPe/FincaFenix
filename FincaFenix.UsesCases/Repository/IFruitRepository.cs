using FincaFenix.Entities.DTOs.FruitDTOs;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Repository
{
    public interface IFruitRepository
    {
        Task<IEnumerable<FruitEntity>> GetAll();
        Task<(IEnumerable<FruitEntity> Fruits, int TotalCount)> GetPaged(FruitFilterDTO filter);
        Task<FruitEntity> GetById(int id);
        Task<bool> Exists(int id);
        Task<FruitEntity> Add(FruitEntity fruit);
        Task<FruitEntity> Update(FruitEntity fruit);
        Task Delete(int id);
    }
}
