using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.UsesCases.Controllers.WorkOrder;

namespace FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder
{
    public class LoadWorkOrdersViewModel(
        IGetWorkOrderInformationController wo)
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public int TotalItems { get; set; } = 0;
        public string? StatusWorkOrderIsDistinct { get; set; } = "Cerrado";
        public IEnumerable<ShowWorkOrderDTO> AllWorkOrderList { get; set; } = new List<ShowWorkOrderDTO>();
        public IEnumerable<ShowWorkOrderDTO> WorkOrderPaginatedList { get; set; } = new List<ShowWorkOrderDTO>();

        public async Task LoadWorkOrderList()
        {
            AllWorkOrderList = await wo.GetAllWorkOrderInfoList();
        }
        public async Task LoadWorkOrderListPaginated()
        {
            var result = await wo.GetWorkOrderListPaginated(CurrentPage, PageSize, StatusWorkOrderIsDistinct);

            WorkOrderPaginatedList = result.Items.ToList();
            TotalItems = result.TotalCount;
        }
        public async Task OnPageChanged(int pageNumber)
        {
            CurrentPage = pageNumber;
            await LoadWorkOrderListPaginated();
        }
    }
}
