using FincaFenix.Entities.DTOs.ShowWorkOrder;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Components.Shared
{
    public partial class StatsCard
    {
        [Parameter] public List<ShowWorkOrderDTO> WorkOrderList { get; set; }
        [Parameter] public EventCallback<FilterWorkOrder> OnFilterSelectedChanged { get; set; }

        private FilterWorkOrder filterWorkOrder = FilterWorkOrder.None;
        public enum FilterWorkOrder
        {
            None,
            Active,
            Pending,
            Closed
        }

        private async Task SetFilterWorkOrder(FilterWorkOrder filter)
        {
            filterWorkOrder = filter;
            await OnFilterSelectedChanged.InvokeAsync(filter);
        }
        private string GetFilterActiveClass(FilterWorkOrder filter)
        {
            return filterWorkOrder == filter ? "active" : "";
        }
    }
}