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

        private int _selectedFarm = 0;
        private int _selectedTask = 0;

        private List<DetailSectorFarmDTO> Sectors { get; set; } = new();

        private List<DetailSectorFarmDTO> ColumnSector1 = new();
        private List<DetailSectorFarmDTO> ColumnSector2 = new();
        private List<DetailSectorFarmDTO> ColumnSector3 = new();
        private List<DetailSectorFarmDTO> ColumnSector4 = new();

        private async Task OnTaskChanged(int taskId)
        {
            _selectedTask = taskId;
            ViewModel.SelectedTaskId = taskId;
        }
        private async Task OnFarmChanged(int farmId)
        {
            _selectedFarm = farmId;

            ColumnSector1.Clear();
            ColumnSector2.Clear();
            ColumnSector3.Clear();
            ColumnSector4.Clear();
            ViewModel.SelectedSectors.Clear();

            if (_selectedFarm != 0)
            {
                await ViewModel.LoadSectorsForFarmIdAsync(_selectedFarm);
                Sectors = ViewModel.Sectors.ToList();
                ViewModel.SelectedFarmId = farmId;
                for (int i = 0; i < Sectors.Count; i++)
                {
                    int columnIndex = i % 4;
                    switch (columnIndex)
                    {
                        case 0:
                            ColumnSector1.Add(Sectors[i]);
                            break;
                        case 1:
                            ColumnSector2.Add(Sectors[i]);
                            break;
                        case 2:
                            ColumnSector3.Add(Sectors[i]);
                            break;
                        case 3:
                            ColumnSector4.Add(Sectors[i]);
                            break;
                    }
                }
            }
            else
            {
                Sectors.Clear();
            }
            await SectorsSelectionChanged.InvokeAsync(ViewModel.SelectedSectors);
        }

        private Task OnSectorSelectionChanged(DetailSectorFarmDTO sector, bool isChecked)
        {
            sector.Selected = isChecked;

            if (isChecked)
            {
                if (!ViewModel.SelectedSectors.Contains(sector))
                    ViewModel.SelectedSectors.Add(sector);
            }
            else
            {
                ViewModel.SelectedSectors.Remove(sector);
            }

            return SectorsSelectionChanged.InvokeAsync(ViewModel.SelectedSectors);
        }
    }
}