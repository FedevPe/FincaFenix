namespace FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder
{
    public interface IGetWorkOrderInformationInputPort
    {
        Task GetWorkOrderById(int id);
        Task GetWorkOrderAndRecipeByIdWorkorder(int id);
        Task GetWorkOrderListPaginated(int pageNumber, int pageSize, string status);
        Task GetAllWorkOrderList();
    }
}
