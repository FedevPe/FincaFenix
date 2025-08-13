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
        public List<ShowWorkOrderDTO> WorkOrderList { get; set; } = new List<ShowWorkOrderDTO>();

        public async Task LoadWorkOrderList()
        {
            WorkOrderList = await wo.GetAllWorkOrderInfoList();
        }
        public async Task LoadWorkOrderListPaginated()
        {
            var (workOrders, totalCount) = await wo.GetWorkOrderListPaginated(CurrentPage, PageSize, StatusWorkOrderIsDistinct);

            WorkOrderList = workOrders.ToList();
            TotalItems = totalCount;
        }
        public async Task OnPageChanged(int pageNumber)
        {
            CurrentPage = pageNumber;
            await LoadWorkOrderListPaginated();
        }
        public async Task LoadFilteredWorkOrdersAsync(SearchBarViewModel filter)
        {
            WorkOrderList.Clear();

            // Aquí implementas la lógica de llamada a tu servicio para filtrar los datos
            // Por ejemplo, puedes tener un método en tu IWorkOrderController para esto.
            // Si no, puedes aplicar el filtro en la lista existente (no recomendado si la lista es grande).

            // Ejemplo de llamada a un nuevo método del controlador:
            // var (workOrders, totalCount) = await wo.GetWorkOrderListFilteredAsync(
            //     filter.SearchTerm, 
            //     filter.FincaId,
            //     filter.TaskId,
            //     filter.State,
            //     filter.StartDate,
            //     filter.EndDate
            // );

            // WorkOrderList = workOrders.ToList();
            // TotalItems = totalCount;

            // Lógica de ejemplo simple:
            var allWorkOrders = await wo.GetAllWorkOrderInfoList(); // O una versión cacheada

            var filteredList = allWorkOrders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                filteredList = filteredList.Where(wo => wo.OrderNum.Contains(filter.SearchTerm));
            }
            if (filter.FincaId > 0)
            {
                filteredList = filteredList.Where(wo => wo.Farm.Id == filter.FincaId);
            }
            if (filter.TaskId > 0)
            {
                filteredList = filteredList.Where(wo => wo.Task.Id == filter.TaskId);
            }
            if (filter.State != "all")
            {
                filteredList = filteredList.Where(wo => wo.Status == filter.State);
            }
            if (filter.StartDate.HasValue)
            {
                filteredList = filteredList.Where(wo => wo.StartDate.Value >= filter.StartDate.Value);
            }
            if (filter.EndDate.HasValue)
            {
                filteredList = filteredList.Where(wo => wo.EndDate.Value <= filter.EndDate.Value);
            }

            WorkOrderList = filteredList.ToList();
            TotalItems = WorkOrderList.Count();
        }
    }
}
