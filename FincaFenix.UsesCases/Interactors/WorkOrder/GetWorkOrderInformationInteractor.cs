using FincaFenix.UsesCases.Interfaces.InputPort.WorkOrder;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;
using FincaFenix.UsesCases.Repository.WorkOrder;

namespace FincaFenix.UsesCases.Interactors.WorkOrder
{
    public class GetWorkOrderInformationInteractor(
        IGetWorkOrderInformationOutputPort presenter,
        IGetWorkOrderInformationRepository repository) : IGetWorkOrderInformationInputPort
    {
        public async Task GetAllWorkOrderList()
        {
            await presenter.HandleList(await repository.GetAllWorkOrderList());
        }

        public async Task GetWorkOrderAndRecipeByIdWorkorder(int id)
        {
            await presenter.Handle(await repository.GetWorkOrderAndRecipeByIdWorkorder(id));
        }

        public async Task GetWorkOrderById(int id)
        {
            await presenter.Handle(await repository.GetWorkOrderById(id));
        }

        public async Task GetWorkOrderListPaginated(int pageNumber, int pageSize, string status)
        {
            var (workOrders, totalCount) = await repository.GetWorkOrderList(pageNumber, pageSize, status);
            await presenter.HandleListPaginated(workOrders, totalCount);
        }
    }
}
