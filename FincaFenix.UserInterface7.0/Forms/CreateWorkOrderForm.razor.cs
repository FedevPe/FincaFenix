using FincaFenix.ViewModels.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Forms
{
    public partial class CreateWorkOrderForm
    {
        [Inject] InfoNewWorkOrderViewModel OrderDataViewModel { get; set; } = default!;
        [Inject] NewRecipeViewModel NewRecipeViewModel { get; set; } = default!;
        [Inject] CreateWorkOrderViewModel CreateWorkOrderViewModel { get; set; } = default!;
        [Inject] NavigationManager NavigationManager { get; set; } = default!;

        private string Description { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await OrderDataViewModel.LoadInitializeAsync();
            await NewRecipeViewModel.LoadInitializeAsync();
        }
        private async Task SaveWorkOrder()
        {
            await CreateWorkOrderViewModel.SetDataWorkOrder(NewRecipeViewModel, OrderDataViewModel);
            // Redireccionar tras guardar
            NavigationManager.NavigateTo("/ordenestrabajo");
        }

    }
}