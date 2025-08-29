using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder.Reports
{
    public partial class WorkedSectorCard
    {
        [Parameter] public DetailSectorFarmDTO SectorFarmDTO { get; set; }
        [Parameter] public int SectorSelectedId { get; set; }
        [Parameter] public EventCallback<int> OnSectorSelectedChanged { get; set; }

        private async Task SetSectorSelected(int id)
        {
            await OnSectorSelectedChanged.InvokeAsync(id);
        }
    }
}