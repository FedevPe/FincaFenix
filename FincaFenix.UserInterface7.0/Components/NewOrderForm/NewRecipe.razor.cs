using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.UserInterface7._0.Services;
using FincaFenix.UserInterface7._0.Validators;
using FincaFenix.ViewModels.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.NewOrderForm
{
    public partial class NewRecipe
    {
        private bool _recipeInitialized = false;

        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private ISnackbar SnackbarService { get; set; }
        [Inject] private TotalAreaSectorService TotalAreaSectorService { get; set; }
        [Parameter] public NewRecipeViewModel ViewModel { get; set; }
        [Inject] public NewRecipeValidator Validator { get; set; }

        private MudForm _form;

        protected override void OnInitialized()
        {
            TotalAreaSectorService.OnChange += StateHasChanged;
        }

        public void Dispose()
        {
            TotalAreaSectorService.OnChange -= StateHasChanged;
        }
        private void OnMachineSelected(int id)
        {
            ViewModel.MachineId = id;
            var machine = ViewModel.MachineList?.FirstOrDefault(m => m.Id == id);
            if (machine is not null)
            {
                ViewModel.Machine = machine;
            }
        }
        private async Task OpenTRVCalculatorDialog()
        {
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, CloseOnEscapeKey = false };
            var dialog = DialogService.Show<CalculatorTRVDialog>("Calculadora TRV", options); // Asegúrate que TRVCalculatorDialog es el nombre de tu componente de diálogo

            var result = await dialog.Result; // Esto espera a que el diálogo se cierre

            if (!result.Canceled) // Si el usuario no canceló el diálogo
            {
                // Intentamos obtener el resultado como un decimal
                if (result.Data is decimal trvValue)
                {
                    ViewModel.TRV = trvValue;
                    StateHasChanged();
                }
            }
        }
        private void AddMaterial()
        {
            if (_recipeInitialized)
            {
                ViewModel.AddMaterial();
            }
        }
        private void RemoveMaterial(DetailRecipeDTO material)
        {
            ViewModel.RemoveMaterial(material);
            if (!ViewModel.Details.Any() && ViewModel.Machine == null) _recipeInitialized = false;
        }
        private void RemoveMachine()
        {
            ViewModel.RemoveMachine();
            ViewModel.Machine.Id = 0;
            if (!ViewModel.Details.Any())
            {
                _recipeInitialized = false; // Deshabilita la sección de receta
            }
        }
        private void AddRecipe()
        {
            if (!_recipeInitialized)
            {
                ViewModel.InitializeRecipe(TotalAreaSectorService.TotalAreaSectors);
                _recipeInitialized = true;
            }
            StateHasChanged();
        }
        private void CalculateEstimatedAmount(DetailRecipeDTO detail)
        {
            try
            {
                if (detail.AmountRequired <= 0)
                {
                    SnackbarService.Add("La 'Cantidad Requerida' no es válida para el cálculo.", MudBlazor.Severity.Warning);
                    return;
                }

                detail.EstimatedAmount = ViewModel.CalculateEstimateAmount(
                    ViewModel.TRV,
                    detail.AmountRequired,
                    TotalAreaSectorService.TotalAreaSectors,
                    detail.AmountRequiredUnit
                );
                if (detail.AmountRequiredUnit == "lts" || detail.AmountRequiredUnit == "cc")
                {
                    detail.EstimatedAmountUnit = "lts";
                }
                else
                {
                    detail.EstimatedAmountUnit = "kg";
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                // Loguear la excepción para depuración (opcional, pero recomendado en aplicaciones reales)
                Console.WriteLine($"Error al calcular la cantidad estimada: {ex.Message}");
                SnackbarService.Add("Ocurrió un error al calcular la cantidad estimada. Verifique los valores ingresados.", MudBlazor.Severity.Error);
                detail.EstimatedAmount = 0;
                StateHasChanged();
            }
        }
    }
}