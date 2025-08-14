using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class LoadWorkOrdersViewModel(
        IWorkOrderController wo)
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public int TotalItems { get; set; } = 0;
        public string? StatusWorkOrderIsDistinct { get; set; } = "Cerrado";
        public List<ShowWorkOrderDTO> AllWorkOrderList { get; set; } = new List<ShowWorkOrderDTO>();
        public List<ShowWorkOrderDTO> WorkOrderPaginatedList { get; set; } = new List<ShowWorkOrderDTO>();

        public async Task LoadWorkOrderList()
        {
            AllWorkOrderList = await wo.GetAllWorkOrderInfoList();
        }
        public async Task LoadWorkOrderListPaginated()
        {
            var (workOrders, totalCount) = await wo.GetWorkOrderListPaginated(CurrentPage, PageSize, StatusWorkOrderIsDistinct);

            WorkOrderPaginatedList = workOrders.ToList();
            TotalItems = totalCount;
        }
        public async Task OnPageChanged(int pageNumber)
        {
            CurrentPage = pageNumber;
            await LoadWorkOrderListPaginated();
        }
        public async Task LoadFilteredWorkOrdersAsync(SearchBarViewModel filter)
        {
            await Task.CompletedTask;
        }
    }
}
