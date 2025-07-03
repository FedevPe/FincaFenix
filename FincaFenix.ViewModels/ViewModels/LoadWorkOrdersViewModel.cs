using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class LoadWorkOrdersViewModel(
        IWorkOrderController wo)
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 4;  
        public int TotalItems { get; set; } = 0;
        public IEnumerable<ShowWorkOrderCardDTO> InfoWorkOrderCard { get; set; } = new List<ShowWorkOrderCardDTO>();

        public async Task LoadWorkOrderList()
        {
            var (workOrders, totalCount) = await wo.GetWorkOrderListPaginated(CurrentPage, PageSize);

            InfoWorkOrderCard = workOrders.ToList();
            TotalItems = totalCount;
        }
        public async Task OnPageChanged(int pageNumber)
        {
            CurrentPage = pageNumber;
            await LoadWorkOrderList();
        }
    }
}
