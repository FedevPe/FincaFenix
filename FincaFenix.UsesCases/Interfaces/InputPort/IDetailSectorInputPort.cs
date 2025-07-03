namespace FincaFenix.UsesCases.Interfaces.Sectors
{
    public interface IDetailSectorInputPort
    {
        Task GetListSectorsByIdFarm(int idFarm);
        Task GetSectorsByOrderId(int orderId);
    }
}
