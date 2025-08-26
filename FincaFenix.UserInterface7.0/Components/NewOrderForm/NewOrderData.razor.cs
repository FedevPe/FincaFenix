using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UserInterface7._0.Services;
using FincaFenix.UserInterface7._0.Validators.WorkOrder;
using FincaFenix.ViewModels.ViewModels.WorkOrder.CreateWorkOrder;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.NewOrderForm
{
    public partial class NewOrderData
    {
        [Inject] public TotalAreaSectorService TotalAreaSectorService { get; set; }
        [Inject] public NewOrderDataUIValidator Validator { get; set; }
        [Inject] public NewDetailSectorUIValidator DetailSectorValidator { get; set; }
        [Parameter] public NewOrderDataViewModel ViewModel { get; set; }

        private MudForm _form;
        public decimal? TotalArea { get; set; } = 0;

        // Método auxiliar para filtrar la lista de cuadros
        private IEnumerable<DetailSectorFarmDTO> GetAvailableSectors(int currentSectorId)
        {
            var selectedIds = ViewModel.SelectedSectors
                                      .Where(s => s.Id != 0 && s.Id != currentSectorId)
                                      .Select(s => s.Id)
                                      .ToList();

            return ViewModel.Sectors.Where(s => !selectedIds.Contains(s.Id));
        }
        private async Task OnFarmChanged(int farmId)
        {
            ViewModel.SelectedFarmId = farmId;
            TotalArea = 0;
            ViewModel.SelectedSectors.Clear();

            if (ViewModel.SelectedFarmId != 0)
            {
                await ViewModel.LoadSectorsForFarmIdAsync(ViewModel.SelectedFarmId);
                AddSectorToSelection(); // Agregamos la primera fila después de cargar los sectores
            }
            StateHasChanged();
        }

        private void OnSectorSelected(DetailSectorFarmDTO currentSector, int selectedId)
        {
            // Resta el área del sector previamente seleccionado (si existe)
            if (currentSector.Selected)
            {
                TotalArea -= currentSector.Area;
            }

            var fullSelectedSector = ViewModel.Sectors.FirstOrDefault(s => s.Id == selectedId);

            if (fullSelectedSector != null && selectedId > 0)
            {
                // Actualiza las propiedades de la fila de la tabla con el nuevo sector seleccionado
                currentSector.Id = fullSelectedSector.Id;
                currentSector.NumberPlants = fullSelectedSector.NumberPlants;
                currentSector.Area = fullSelectedSector.Area;
                currentSector.SectorName = fullSelectedSector.SectorName;
                currentSector.FruitName = fullSelectedSector.FruitName;
                currentSector.VarietyName = fullSelectedSector.VarietyName;
                currentSector.Selected = true;

                // Suma el área del nuevo sector
                TotalArea += fullSelectedSector.Area;
            }
            else
            {
                // Si se deselecciona o se selecciona la opción "Seleccione un cuadro"
                // Restablece los valores
                currentSector.Id = 0;
                currentSector.NumberPlants = 0;
                currentSector.Area = 0;
                currentSector.SectorName = null;
                currentSector.FruitName = null;
                currentSector.VarietyName = null;
                currentSector.Selected = false;
            }

            TotalAreaSectorService.SetTotalAreaSectors(TotalArea);
            ViewModel.TotalAreaSectors = TotalArea;
            StateHasChanged();
        }

        private void AddSectorToSelection()
        {
            // Agrega una nueva fila al modelo de datos
            ViewModel.SelectedSectors.Add(new DetailSectorFarmDTO());
        }

        private void RemoveSectorFromSelection(DetailSectorFarmDTO sector)
        {
            if (ViewModel.SelectedSectors.Contains(sector))
            {
                // Resta el área solo si la fila tenía un sector seleccionado
                if (sector.Selected)
                {
                    TotalArea -= sector.Area;
                }
                ViewModel.SelectedSectors.Remove(sector);
                TotalAreaSectorService.SetTotalAreaSectors(TotalArea);
                ViewModel.TotalAreaSectors = TotalArea;
            }
            StateHasChanged();
        }
    }
}