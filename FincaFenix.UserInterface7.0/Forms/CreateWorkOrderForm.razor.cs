using FincaFenix.UserInterface7._0.Validators;
using FincaFenix.ViewModels.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Forms
{
    public partial class CreateWorkOrderForm
    {
        [Inject] public CreateWorkOrderViewModel CreateWorkOrderViewModel { get; set; } = default!;
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public ISnackbar Snackbar { get; set; } = default!;
        [Inject] public CreateWorkOrderValidator Validator { get; set; } = default!;

        private MudForm createWorkOrderForm;

        protected override async Task OnInitializedAsync()
        {
            await CreateWorkOrderViewModel.OrderData.LoadInitializeAsync();
            await CreateWorkOrderViewModel.RecipeData.LoadInitializeAsync();
        }
        private async Task SaveWorkOrder()
        {
            // Validar el formulario completo
            await createWorkOrderForm.Validate();

            // Si el formulario es válido (incluyendo los sub-ViewModels a través de los validadores encadenados)
            if (createWorkOrderForm.IsValid)
            {
                // Mapear y guardar los datos usando el ViewModel principal
                await CreateWorkOrderViewModel.SetDataWorkOrder();
                // Redireccionar tras guardar
                NavigationManager.NavigateTo("/ordenestrabajo");
            }
            else
            {
                Snackbar.Add("Por favor, completá todos los campos requeridos", Severity.Error);
            }
        }
    }
}