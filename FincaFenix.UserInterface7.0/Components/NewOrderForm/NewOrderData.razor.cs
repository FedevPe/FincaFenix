using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UserInterface7._0.Services;
using FincaFenix.UserInterface7._0.Validators.WorkOrder;
using FincaFenix.ViewModels.ViewModels;
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
        private async Task OnFarmChanged(int farmId)
        {
            ViewModel.SelectedFarmId = farmId;
            TotalArea = 0;
            ViewModel.SelectedSectors.Clear();
            AddSectorToSelection();

            if (ViewModel.SelectedFarmId != 0)
            {
                await ViewModel.LoadSectorsForFarmIdAsync(ViewModel.SelectedFarmId);
                ViewModel.SelectedFarmId = farmId;
            }
            else
            {
                ViewModel.Sectors.ToList().Clear();
            }
            StateHasChanged();
        }
        private void OnSectorSelected(DetailSectorFarmDTO currentSector, int selectedId)
        {
            decimal? oldArea = currentSector.Area;
            var fullSelectedSector = ViewModel.Sectors.FirstOrDefault(s => s.Id == selectedId);
            //ViewModel.Sectors.ToList().Remove(fullSelectedSector);

            if (fullSelectedSector != null && selectedId > 0) // Asegúrate de que no sea la opción "Seleccione un cuadro"
            {
                if (currentSector.Selected) // Asume que 'Selected' indica que era un sector previamente válido
                {
                    TotalArea -= oldArea;
                }
                // Si oldArea es 0 y currentSector.Selected es false, significa que era una fila nueva o sin selección previa,
                // así que no restamos nada.
                TotalArea += fullSelectedSector.Area;
                TotalAreaSectorService.SetTotalAreaSectors(TotalArea);
                // Actualiza las propiedades del sector en SelectedSectors
                currentSector.Id = fullSelectedSector.Id;
                currentSector.NumberPlants = fullSelectedSector.NumberPlants;
                currentSector.Area = fullSelectedSector.Area;
                currentSector.SectorName = fullSelectedSector.SectorName; // Asegúrate de copiar todas las propiedades relevantes
                currentSector.FruitName = fullSelectedSector.FruitName;
                currentSector.VarietyName = fullSelectedSector.VarietyName;
                currentSector.Selected = true; // Marca este sector como seleccionado
            }
            else
            {
                // Si se selecciona la opción "Seleccione un cuadro" (Value="0")
                // o si por alguna razón no se encuentra el sector,
                // restablece los valores y marca como no seleccionado.
                currentSector.Id = 0;
                currentSector.NumberPlants = 0;
                currentSector.Area = 0;
                currentSector.Selected = false;
            }

            // Si tu ViewModel no es reactivo automáticamente,
            // podrías necesitar llamar a StateHasChanged(); si estás en un componente Blazor Server/WASM.
            StateHasChanged(); // Forzamos una actualización de la UI
        }
        private void AddSectorToSelection()
        {
            ViewModel.SelectedSectors.Add(new DetailSectorFarmDTO());
        }
        private void RemoveSectorFromSelection(DetailSectorFarmDTO sector)
        {
            if (ViewModel.SelectedSectors.Contains(sector))
            {
                TotalArea -= sector.Area;
                TotalAreaSectorService.SetTotalAreaSectors(TotalArea);
                ViewModel.SelectedSectors.Remove(sector);
            }
            StateHasChanged();
        }
    }
}