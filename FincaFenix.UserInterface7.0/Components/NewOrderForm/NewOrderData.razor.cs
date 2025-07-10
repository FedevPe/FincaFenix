using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.ViewModels.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Components.NewOrderForm
{
    public partial class NewOrderData
    {
        [Parameter]
        public EventCallback<List<DetailSectorFarmDTO>> SectorsSelectionChanged { get; set; }
        [Parameter]
        public InfoNewWorkOrderViewModel ViewModel { get; set; }

        private List<DetailSectorFarmDTO> Sectors { get; set; } = new();
        
        private async Task OnFarmChanged(int farmId)
        {
            ViewModel.SelectedFarmId = farmId;
            ViewModel.SelectedSectors.Clear();

            if (ViewModel.SelectedFarmId != 0)
            {
                await ViewModel.LoadSectorsForFarmIdAsync(ViewModel.SelectedFarmId);
                Sectors = ViewModel.Sectors.ToList();
                ViewModel.SelectedFarmId = farmId;
            }
            else
            {
                Sectors.Clear();
            }
            await SectorsSelectionChanged.InvokeAsync(ViewModel.SelectedSectors);
        }
    }
}